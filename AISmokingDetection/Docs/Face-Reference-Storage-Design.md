# Face Reference Storage Design --- AISmokingDetection

**Status:** Draft --- preparation and design only\
**Implementation status:** Not implemented\
**Target storage:** StackShift S2 Object Storage private bucket\
**Backend:** ASP.NET Core Web API, .NET 10, EF Core 10

## 1. Purpose

Move face-reference image storage from the backend machine's local
filesystem to StackShift S2 Object Storage. SQL Server will store image metadata and
the storage key. The backend will control authorized access for the AI
service.

This is a proposed design only. No S2 account, bucket, credentials,
upload integration, or AI integration has been created yet.

## 2. Current implementation

The current `FaceReferenceController`: - Accepts an image through
`IFormFile`. - Limits image size to 5 MB. - Allows `.jpg`, `.jpeg`, and
`.png`. - Checks the JPEG/PNG file signature. - Generates a random
GUID-based filename. - Saves the image under
`LocalApplicationData/AISmokingDetection/FaceStorage`. - Saves
`ParticipantId`, `StorageKey`, `CreatedAt`, and `IsActive` in
`FaceReferenceMetadata`.

The current implementation uses local filesystem storage. It does not
upload to S2 or generate temporary URLs.

## 3. Proposed architecture

``` text
FaceReferenceController
        |
        v
IImageStorageService
        |
        v
S2ImageStorageService
        |
        v
Private StackShift S2 bucket

FaceReferenceController
        |
        v
AppDbContext
        |
        v
FaceReferenceMetadata (SQL Server)
```

Responsibilities: - `FaceReferenceController`: HTTP validation,
authorization, coordination, and response shaping. -
`IImageStorageService`: provider-independent storage abstraction. -
`S2ImageStorageService`: S2-specific upload, delete, and temporary URL
operations. - `AppDbContext`: persist metadata and the S2 object key.

## 4. Data storage

### SQL Server

Keep `FaceReferenceMetadata`. Its `StorageKey` should contain the object
key within the bucket, not a permanent public URL and not image bytes.

Illustrative object key:

``` text
participants/123/face-references/7f2c...e91.jpg
```

The backend generates the key. Do not use the uploaded filename as the
storage key.

### StackShift S2 Object Storage

S2 is an S3-compatible object storage service. The backend should use
an S3-compatible client and AWS Signature Version 4 authentication.

The endpoint and signing region must come from the StackShift API
for the created bucket. Do not assume AWS endpoints or default regions.

-   Use a private bucket.
-   Do not enable public access for face-reference images.
-   Keep access credentials on the backend only.
-   Never commit access keys, secret keys, or connection strings to
    source control.

The bucket has not yet been created.

## 5. Proposed workflows

### 5.1 Enrollment

1.  An authorized client submits the participant identifier and image.
2.  Backend verifies that the participant exists and is active.
3.  Backend validates file size, allowed image type, and file signature.
4.  Backend generates a unique object key.
5.  Backend uploads the image to the private S2 bucket.
6.  Backend saves a `FaceReferenceMetadata` row with the participant ID
    and object key.
7.  Backend provides the AI service with a short-lived URL, or another
    agreed secure delivery mechanism, to process the image.

If upload succeeds but the database write fails, backend should attempt
to delete the uploaded object and log cleanup failures.

### 5.2 Recognition

When AI needs a stored reference image later:

1.  AI requests access through an authenticated backend-to-backend API.
2.  The request identifies one image using a stable identifier such as
    `FaceReferenceMetadataId`.
3.  Backend checks caller authorization and confirms the metadata and
    participant are active.
4.  Backend generates a new short-lived URL.
5.  Backend returns the URL and its expiration time to the authorized AI
    service.

Do not expose a public bucket or give AI permanent bucket credentials
merely to retrieve reference images.

## 6. Proposed service contract

Illustrative interface; not yet added to the codebase:

``` csharp
public interface IImageStorageService
{
    Task UploadAsync(
        string storageKey,
        Stream content,
        string contentType,
        CancellationToken cancellationToken);

    Task<string> GetTemporaryUrlAsync(
        string storageKey,
        TimeSpan expiresIn,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken);
}
```

Finalize exact signatures and return types during implementation.

## 7. Proposed API contract

These are proposed routes, not implemented endpoints.

  ----------------------------------------------------------------------------------------------
  Method                  Proposed route                                 Purpose
  ----------------------- ---------------------------------------------- -----------------------
  `POST`                  `/api/FaceReference/{participantId}`           Upload and register a
                                                                         face-reference image

  `GET`                   `/api/FaceReference/{metadataId}/access-url`   Generate a temporary
                                                                         URL for one active
                                                                         image

  `DELETE`                `/api/FaceReference/{metadataId}`              Deactivate metadata and
                                                                         remove the stored
                                                                         object under the agreed
                                                                         retention policy
  ----------------------------------------------------------------------------------------------

The access-URL endpoint should be restricted to an authenticated AI
service identity, not arbitrary authenticated users.

## 8. Security and privacy requirements

Face images are biometric data and require careful handling.

-   Use a private S2 bucket.
-   Require authentication and role/service-level authorization.
-   Use short-lived, narrowly scoped presigned URLs.
-   Never log presigned URLs, image contents, or credentials.
-   Validate content type and file signature; do not trust
    client-provided filenames or MIME types.
-   Apply upload size limits.
-   Generate object keys on the backend.
-   Define who may enroll, view, deactivate, and delete reference
    images.
-   Agree on consent/legal basis, purpose limitation, retention period,
    and deletion process with the project supervisor/team and applicable
    requirements.
-   Decide whether AI downloads immediately or needs a URL for later
    retrieval.

## 9. Configuration plan

Illustrative configuration keys only; do not place actual secrets in
this file:

``` json
{
  "S2": {
    "Endpoint": "",
    "Region": "",
    "BucketName": "",
    "AccessKeyId": "",
    "SecretAccessKey": ""
  }
}
```

Use .NET User Secrets during development and secure environment
configuration or a secret store in deployment. Do not commit
credentials.

## 10. Decisions and open questions

  Item                                     Current position
  ---------------------------------------- ---------------------------------------------
  Storage provider                         StackShift S2 Object Storage proposed
  Bucket access                            Private
  Backend uploads images                   Agreed in current discussion
  AI needs images during Enrollment        Assumed yes
  AI needs images during Recognition       Assumed yes
  Database stores                          `StorageKey` and metadata
  Permanent public image URLs              Not used
  Temporary URL expiry                     To be agreed; use shortest practical period
  AI authentication mechanism              To be agreed
  One or multiple images per participant   To be confirmed
  Image replacement/deactivation policy    To be agreed
  Retention and deletion policy            To be agreed
  S2 account and bucket                    Not created

## 11. Implementation checklist

-   [ ] Confirm AI service-to-backend authentication.
-   [ ] Confirm whether a participant can have multiple active reference
    images.
-   [ ] Agree on URL lifetime and when AI fetches the image.
-   [ ] Agree on retention, deactivation, and deletion behavior.
-   [ ] Create and configure a private S2 bucket.
-   [ ] Store S2 credentials securely.
-   [ ] Add `IImageStorageService`.
-   [ ] Implement `S2ImageStorageService`.
-   [ ] Register the service in `Program.cs`.
-   [ ] Update `FaceReferenceController` to use the storage abstraction.
-   [ ] Add an authorized temporary-URL endpoint or equivalent AI
    integration.
-   [ ] Add tests for validation, authorization, persistence, cleanup,
    URL generation, and deletion.
-   [ ] Document the agreed API contract for the AI team.

## 12. Current completion boundary

Completed in preparation: - Reviewed `Program.cs`, `AppDbContext`,
`ParticipantController`, `FaceReferenceController`, entity definitions,
and project package references. - Identified local filesystem storage as
the current implementation. - Outlined the target StackShift S2 architecture and
Enrollment/Recognition workflows.

Not completed: - S2 account activation or bucket creation. - S2
credentials or configuration. - S2-compatible client package installation. - S2 service
implementation. - AI API contract confirmation. - Uploading or
retrieving any image from S2.

**This document is a proposed technical design only.**
