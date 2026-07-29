# Google OAuth en Mentorly

Este documento explica como funciona la autenticacion con Google OAuth en Mentorly, como se integra con ASP.NET Core Identity y como se mapea el usuario autenticado al modelo `Student` del dominio.

## 1. Resumen de arquitectura

Mentorly usa:

- Blazor Server (.NET 10) como capa de presentacion.
- ASP.NET Core Identity con EF Core (SQLite) para sesion y usuarios autenticados.
- Google OAuth como proveedor externo de login.
- Mapeo de claims de Google al agregado de dominio `Student`.

Cuando un usuario inicia sesion con Google:

1. Se autentica contra Google.
2. Identity crea o vincula una cuenta local (`ApplicationUser`).
3. Se crea o actualiza un `Student` en la base de datos de dominio.
4. Se agrega claim `mentorly_student_id` para relacionar UI/servicios con el estudiante real.

## 2. Configuracion requerida

Completa estos valores en configuracion:

```json
"Authentication": {
  "Google": {
    "ClientId": "TU_CLIENT_ID",
    "ClientSecret": "TU_CLIENT_SECRET"
  }
}
```

Archivo sugerido para desarrollo local:

- `appsettings.Development.json`

Recomendado para no versionar secretos:

```powershell
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "TU_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "TU_CLIENT_SECRET"
```

## 3. Endpoints de autenticacion

La app expone estos endpoints:

- `GET /auth/login?returnUrl=/ruta`
  - Inicia challenge contra Google.
- `GET /auth/callback?returnUrl=/ruta`
  - Recibe identidad externa, crea/vincula `ApplicationUser`, sincroniza `Student`, agrega claim `mentorly_student_id`, crea cookie de sesion.
- `GET /auth/logout?returnUrl=/`
  - Cierra sesion local (cookie Identity).

## 4. Flujo completo paso a paso

### 4.1 Login

1. Usuario pulsa "Sign in with Google" en el menu.
2. Navegador va a `/auth/login`.
3. El backend ejecuta `ChallengeAsync` con esquema `Google`.
4. Google autentica y redirige a `/auth/callback`.

### 4.2 Callback

En `/auth/callback`:

1. Se leen claims de Google (`NameIdentifier`, `Email`, `Name`).
2. Se busca `ApplicationUser` por login Google o por email.
3. Si no existe, se crea.
4. Si falta el login externo Google, se vincula con `AddLoginAsync`.
5. Se ejecuta mapeo de dominio:
   - `IStudentIdentityMapper.EnsureStudentAsync(principal)`.
   - Si no existe `Student` por `GoogleUserId`, se crea.
   - Si existe, se actualizan `Email` y `DisplayName`.
6. Se persiste claim `mentorly_student_id` en Identity.
7. Se hace `SignInAsync` persistente y redireccion al `returnUrl`.

### 4.3 Uso de claims en Blazor

En componentes Blazor, el `StudentId` se lee desde claims para evitar IDs hardcodeados:

- claim de dominio: `mentorly_student_id`
- fuente de estado: `AuthenticationStateProvider`

Esto permite que servicios de aplicacion operen con el estudiante autenticado real.

## 5. Clases clave

- `Program.cs`
  - Configura Identity, cookies, Google OAuth, authorization y endpoints `/auth/*`.
- `ApplicationUser`
  - Entidad de Identity para usuario autenticado.
- `MentorlyDbContext`
  - DbContext de dominio + tablas Identity (hereda de `IdentityDbContext`).
- `IStudentIdentityMapper`
  - Contrato para sincronizar claims -> `Student`.
- `StudentIdentityMapper`
  - Implementacion que crea/actualiza `Student` desde claims.
- `MentorlyClaimTypes`
  - Define claim `mentorly_student_id`.

## 6. Comportamiento en UI

En `NavMenu`:

- Si no hay sesion: muestra boton "Sign in with Google".
- Si hay sesion: muestra usuario autenticado + boton "Sign out".

En `ExerciseSubmission`:

- Lee `mentorly_student_id` en `OnInitializedAsync`.
- Si no existe claim, solicita iniciar sesion.
- Si existe, usa ese `StudentId` para operaciones de inscripcion y envio.

## 7. Errores comunes y solucion

### Error: "Google OAuth is not configured"

Causa:

- Faltan `Authentication:Google:ClientId` y/o `ClientSecret`.

Solucion:

- Configurar ambos valores en `appsettings.Development.json` o User Secrets.

### Error en callback por claims faltantes

Causa:

- Google no devolvio claims minimos o scopes incompletos.

Solucion:

- Verificar configuracion de app en Google Cloud y permisos de email/profile.

### No aparece `mentorly_student_id`

Causa:

- Fallo en mapeo de `Student` durante callback.

Solucion:

- Revisar logs del endpoint `/auth/callback` y persistencia en DB.

## 8. Seguridad recomendada

- No subir `ClientSecret` a git.
- Usar User Secrets en desarrollo y variables de entorno en produccion.
- Mantener HTTPS habilitado.
- Revisar periodicamente dependencias con advertencias de seguridad.

## 9. Checklist rapido de puesta en marcha

1. Configurar credenciales Google.
2. Ejecutar la app.
3. Ir a `Sign in with Google`.
4. Confirmar login correcto y presencia de claim `mentorly_student_id`.
5. Probar `Exercise Submission` con usuario autenticado.
