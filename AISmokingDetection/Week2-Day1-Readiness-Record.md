# Week 2 - Day 1 Readiness Record

## 1. Current Implementation

- Participant entity exists.
- FaceReferenceMetadata entity exists.
- Participant and FaceReferenceMetadata have a one-to-many relationship.
- Reference images are stored outside the project public assets.
- StorageKey is saved in the database.
- Upload endpoint validates file size, extension and image signature.
- Participant enrollment endpoint exists.
- JWT authentication is implemented.
- Administrator and Reviewer roles exist.

## 2. Identified Gaps

- Enrollment endpoints currently allow any authenticated staff member.
- Face reference listing endpoint is missing.
- Face reference revocation endpoint is missing.
- Enrollment processing states are not implemented.
- Gallery version metadata is not implemented.
- AI shared storage access contract is not finalized.
- Retention and deletion policy is not finalized.

## 3. BE-A Proposed Decisions - Pending Team Approval

### Enrollment Permissions
- Administrator only can create participants and upload reference images.
- Reviewer may have read-only access to required participant information.

### AI Access
- AI should access reference images through a secured internal storage mechanism.
- Raw reference images must not be exposed through public assets or ordinary client APIs.
- StorageKey should be used as an opaque reference.

### Revocation
- Revocation should preserve metadata instead of immediately deleting the record.
- Revoked references must be excluded from future gallery builds.
- AI must define how previously generated embeddings and gallery versions are handled.

## 4. Team Agreements Required

### BE-B
- Confirm Participant schema compatibility.
- Confirm any required relationships or identifiers.

### AI
- Confirm shared storage architecture and access mechanism.
- Confirm enrollment processing states.
- Confirm gallery versioning and revoked-reference handling.
- Confirm ownership of processing status updates.

### WEB / MOB
- Confirm which participant and enrollment operations their clients require.
- Confirm that restricted operations remain unavailable to unauthorized roles.

## 5. Proposed Enrollment States

- Pending
- Ready
- Failed
- Revoked

These states are proposals pending AI and team approval.

## 6. Completion Checklist

- [ ] Role matrix approved.
- [ ] BE-B schema agreement confirmed.
- [ ] AI storage access contract confirmed.
- [ ] Enrollment states and ownership confirmed.
- [ ] Revocation and retention policy agreed.
- [ ] API contract documented.

## 7. Current Status

IN PROGRESS - Implementation review completed.
Team agreements are still pending.

No entity changes or migrations should be created until the required agreements are confirmed.
