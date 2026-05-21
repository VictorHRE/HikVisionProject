# React + TypeScript + Vite

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

## React Compiler

The React Compiler is not enabled on this template because of its impact on dev & build performances. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).

## Expanding the ESLint configuration

If you are developing a production application, we recommend updating the configuration to enable type-aware lint rules:

```js
export default defineConfig([
  globalIgnores(["dist"]),
  {
    files: ["**/*.{ts,tsx}"],
    extends: [
      // Other configs...

      // Remove tseslint.configs.recommended and replace with this
      tseslint.configs.recommendedTypeChecked,
      // Alternatively, use this for stricter rules
      tseslint.configs.strictTypeChecked,
      // Optionally, add this for stylistic rules
      tseslint.configs.stylisticTypeChecked,

      // Other configs...
    ],
    languageOptions: {
      parserOptions: {
        project: ["./tsconfig.node.json", "./tsconfig.app.json"],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
]);
```

You can also install [eslint-plugin-react-x](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-x) and [eslint-plugin-react-dom](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-dom) for React-specific lint rules:

```js
// eslint.config.js
import reactX from "eslint-plugin-react-x";
import reactDom from "eslint-plugin-react-dom";

export default defineConfig([
  globalIgnores(["dist"]),
  {
    files: ["**/*.{ts,tsx}"],
    extends: [
      // Other configs...
      // Enable lint rules for React
      reactX.configs["recommended-typescript"],
      // Enable lint rules for React DOM
      reactDom.configs.recommended,
    ],
    languageOptions: {
      parserOptions: {
        project: ["./tsconfig.node.json", "./tsconfig.app.json"],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
]);
```

## Consumir una API externa

Este proyecto usa tipos locales en `src/services/types.ts` y `src/services/api.ts` es la fuente de datos; el servicio local `src/services/storage.ts` y su demo fueron removidos para priorizar el uso de la API.

Pasos rápidos:

- Agrega `VITE_API_URL` en un archivo `.env.local` o `.env` en la raíz. (También está `.env.example` para referencia.)
- Reinicia el servidor de desarrollo `npm run dev` para que `import.meta.env.VITE_API_URL` se cargue.

Proxy en desarrollo (evitar CORS/SSL)

- Se añadió una configuración de proxy en `vite.config.ts` que, si `VITE_API_URL` está presente en `.env.local`, mapeará las rutas que comiencen con `/api` al origen del backend. Esto evita errores de CORS y los problemas de certificados autofirmados en HTTPS.
- Ejemplo: si `VITE_API_URL=https://192.168.1.28:45456/api/authentication`, una petición a `/api/authentication/login` en la app de desarrollo será proxiada a `https://192.168.1.28:45456/api/authentication/login`.
- Alternativa para usar proxy (recomendado durante el desarrollo): en `.env.local` pon `VITE_API_URL=/api`. Con esto todas las llamadas a rutas que empiecen con `/api` irán al backend por medio del proxy configurado en `vite.config.ts`, evitando así CORS y problemas de certificados en desarrollo.

Login path personalizado

- Si tu API tiene una ruta de login distinta, añade `VITE_LOGIN_PATH` a tu `.env.local`. Ejemplo:

  VITE_LOGIN_PATH=/api/authentication/login

Nota: `VITE_LOGIN_PATH` debe ser relativo a `VITE_API_URL` cuando `VITE_API_URL` contiene la ruta base completa.
Por ejemplo, si `VITE_API_URL=https://host:7014/api/authentication`, entonces configura `VITE_LOGIN_PATH=/login`.
Si tu `VITE_API_URL` apunta a `/api` para usar el proxy, entonces configura `VITE_LOGIN_PATH=/api/authentication/login`.

Esto hará que la app use exactamente esa ruta para el POST de login y evitará que intente rutas alternativas.

- No olvides reiniciar el servidor de desarrollo después de cambiar `.env.local`.

Comportamiento:

- Si `VITE_API_URL` está configurado, las páginas (Empleados, Sucursales, Historial) intentarán usar la API.
- Si la API no responde o falla en estos endpoints, la app usará un fallback local (almacenado en `localStorage`) para las operaciones principales de Empleados, Sucursales e Historial. Esto permite que la UI funcione aún sin back-end completo.

Funciones disponibles en `src/services/api.ts` (mapeadas a los endpoints comunes):

- getEmployees(): GET /employees
- addEmployee(emp): POST /employees
- saveEmployees(employees): PUT /employees (bulk replace)
- addFingerprint(employeeId, data): POST /employees/:id/fingerprints (json fallback)
- uploadFingerprint(employeeId, file): POST /employees/:id/fingerprints (multipart/form-data)
- removeFingerprint(employeeId, index): DELETE /employees/:id/fingerprints/:index
- toggleEmployeeActive(employeeId): POST /employees/:id/toggle
- getBranches(): GET /branches
- saveBranches(branches): PUT /branches
- toggleBranchActive(branchId): POST /branches/:id/toggle
- getHistory(filters): GET /history?employeeId=...&from=...
- Autenticación (get token): POST /api/authentication/gettoken con body:
  { id: 0, name: string, email: string, password: string }
  Si la respuesta contiene `{ isSuccess: true, token: "..." }` se guarda en `localStorage` como `fp_token` y se añade `Authorization: Bearer <token>` automáticamente a todas las peticiones.
- addHistory(entry): POST /history

Nota: la API debe devolver objetos con las mismas propiedades que los tipos definidos en `src/services/types.ts`. Si el backend responde 401/403, el cliente limpia su token y requiere que el usuario inicie sesión nuevamente.
