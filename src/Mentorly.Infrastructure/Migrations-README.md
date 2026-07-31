# Entity Framework Core Migrations - Guía

Este documento explica cómo trabajar con migraciones de Entity Framework Core en el proyecto Mentorly.

## Configuración

El proyecto está configurado con:
- **Provider**: SQL Server
- **Design-Time Factory**: `MentorlyDbContextFactory` en `src/Mentorly.Infrastructure/Persistence/`
- **Connection String**: Definida en `src/Mentorly.UI/appsettings.json`

## Comandos de Migraciones

### Crear una nueva migración

Desde la raíz del proyecto Infrastructure:

```powershell
cd D:\AplicadaI\Mentorly.Net\src\Mentorly.Infrastructure
dotnet ef migrations add <NombreMigracion> --startup-project ..\Mentorly.UI\Mentorly.UI.csproj
```

O desde la raíz de la solución:

```powershell
dotnet ef migrations add <NombreMigracion> --project src/Mentorly.Infrastructure --startup-project src/Mentorly.UI
```

### Aplicar migraciones a la base de datos

```powershell
cd D:\AplicadaI\Mentorly.Net\src\Mentorly.Infrastructure
dotnet ef database update --startup-project ..\Mentorly.UI\Mentorly.UI.csproj
```

### Eliminar la última migración (si no se ha aplicado)

```powershell
cd D:\AplicadaI\Mentorly.Net\src\Mentorly.Infrastructure
dotnet ef migrations remove --startup-project ..\Mentorly.UI\Mentorly.UI.csproj
```

### Eliminar la última migración (forzar)

```powershell
cd D:\AplicadaI\Mentorly.Net\src\Mentorly.Infrastructure
dotnet ef migrations remove --startup-project ..\Mentorly.UI\Mentorly.UI.csproj --force
```

### Listar migraciones

```powershell
cd D:\AplicadaI\Mentorly.Net\src\Mentorly.Infrastructure
dotnet ef migrations list --startup-project ..\Mentorly.UI\Mentorly.UI.csproj
```

### Generar script SQL de migraciones

```powershell
cd D:\AplicadaI\Mentorly.Net\src\Mentorly.Infrastructure
dotnet ef migrations script --startup-project ..\Mentorly.UI\Mentorly.UI.csproj --output migration.sql
```

## Notas Importantes

1. **Design-Time Factory**: El `MentorlyDbContextFactory` se utiliza automáticamente en tiempo de diseño cuando ejecutas comandos de EF Core Tools.

2. **Connection String**: El factory lee la cadena de conexión desde `appsettings.json` del proyecto UI. Asegúrate de que la cadena de conexión sea válida antes de ejecutar comandos de migración.

3. **SQL Server**: El proyecto está configurado para usar SQL Server. La cadena de conexión por defecto es:
   ```
   Server=localhost;Database=MentorlyDb_Dev;Integrated Security=true;TrustServerCertificate=true;
   ```

4. **Múltiples Entornos**: 
   - Desarrollo: `MentorlyDb_Dev` (desde appsettings.Development.json)
   - Producción: `MentorlyDb` (desde appsettings.json)

## Solución de Problemas

### Error: "Unable to create a DbContext of type 'MentorlyDbContext'"

Verifica que:
- El paquete `Microsoft.EntityFrameworkCore.Design` esté instalado en los proyectos Infrastructure y UI
- La cadena de conexión en `appsettings.json` sea correcta
- SQL Server esté en ejecución

### Error: "Unable to retrieve project metadata"

Asegúrate de ejecutar los comandos desde el directorio correcto o usar rutas absolutas.
