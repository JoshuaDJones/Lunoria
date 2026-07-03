# API Layer

The frontend API contracts are derived from the current ASP.NET controllers,
request models, and DTOs.

## Shared Infrastructure

- `src/lib/apiClient.ts` configures Axios, bearer authentication, and API error
  handling.
- `src/lib/formData.ts` converts typed inputs into multipart form data.
- `src/lib/imagesApi.ts` contains the shared image upload and deletion calls.

Authentication uses the `auth_token` local-storage key for compatibility with
the previous frontend.

## Feature APIs

Each domain exports its models and API functions from its feature root:

```ts
import {
  createCharacter,
  listCharacters,
  type Character,
} from "@/features/characters";
```

Available feature entry points:

- `@/features/auth`
- `@/features/characters`
- `@/features/equipment`
- `@/features/items`
- `@/features/journeys`
- `@/features/playthroughs`
- `@/features/scenes`
- `@/features/spells`

## Errors

API calls reject when the server returns a non-success status. Convert unknown
errors into the backend error shape with:

```ts
import { getApiError } from "@/lib/apiClient";

try {
  await createSomething(input);
} catch (error) {
  const apiError = getApiError(error);
  console.error(apiError.code, apiError.message);
}
```

## Files and Forms

Create inputs requiring an image use a required `File`. Update inputs use an
optional `File`, allowing the existing image to remain unchanged.

Do not manually set the multipart `Content-Type` header. Axios adds the correct
boundary when it receives a `FormData` instance.

## Dates and Enums

- ASP.NET `DateTime` values are represented as ISO date strings.
- Numeric enums match the backend enum values exactly.
- Convert date strings into `Date` or Luxon values at the presentation
  boundary rather than inside the API layer.
