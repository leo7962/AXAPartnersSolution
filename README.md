# AXA Partners - Sistema de Gestión de Usuarios y Departamentos

## 📋 Descripción

Sistema de gestión desarrollado en WPF (.NET 9) para la administración de usuarios y asignación a departamentos, implementando patrones de arquitectura modernos como Clean Architecture, CQRS y MVVM.

## 🚀 Características Principales

- ✅ **Gestión completa de usuarios** (CRUD)
- ✅ **Asignación de usuarios a departamentos**
- ✅ **Visualización de asignaciones actuales**
- ✅ **Validación en tiempo real**
- ✅ **Interfaz moderna y responsiva**
- ✅ **Patrón CQRS con MediatR**
- ✅ **Entity Framework Core con SQL Server**
- ✅ **Pruebas unitarias completas**
- ✅ **Inyección de dependencias con Microsoft.Extensions**

## 🛠️ Requisitos Previos

### Software Necesario:
- [**.NET 9 SDK**](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server** (LocalDB incluido con Visual Studio)
- **Visual Studio 2022** (recomendado) o VS Code
- **Git** (para clonar el repositorio)

### Herramientas Opcionales:
- **SQL Server Management Studio** (SSMS)
- **Postman** (para probar APIs si se extiende el proyecto)

## 📁 Estructura del Proyecto

```
AXAPartnersSolution/
├── src/
│   ├── Presentation/          # Capa de presentación (WPF + MVVM)
│   ├── Application/           # Lógica de aplicación (CQRS)
│   ├── Domain/                # Entidades y reglas de negocio
│   ├── Infrastructure/        # Persistencia (EF Core)
│   └── Shared/                # Componentes compartidos
├── database/
│   └── Scripts/              # Scripts SQL
└── README.md                 # Este archivo
```

## 🚀 Configuración Inicial Rápida

### 1. Clonar y Restaurar Dependencias

```bash
# Clonar el repositorio (si aplica)
git clone <url-del-repositorio>
cd AXAPartnersSolution

# Restaurar paquetes NuGet
dotnet restore
```

### 2. Configurar la Base de Datos

#### Opción A: Usar migraciones automáticas (Recomendado)

La aplicación creará automáticamente la base de datos al iniciar por primera vez. Asegúrate de que LocalDB esté instalado:

```bash
# Verificar que LocalDB esté disponible
sqllocaldb info
```

#### Opción B: Crear base de datos manualmente

```sql
-- Ejecutar en SQL Server Management Studio
CREATE DATABASE AXAPartnersDB;
GO

USE AXAPartnersDB;
GO

-- Las tablas se crearán automáticamente al ejecutar la aplicación
```

### 3. Configurar Connection String

Editar `Presentation/appsettings.json` si necesitas cambiar la cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AXAPartnersDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

## ▶️ Ejecutar la Aplicación

### Desde Visual Studio:
1. Abrir `AXAPartnersSolution.sln`
2. Establecer `Presentation` como proyecto de inicio
3. Presionar **F5** o **Ctrl + F5**

### Desde Terminal:
```bash
# Navegar al proyecto Presentation
cd src/Presentation

# Ejecutar la aplicación
dotnet run
```

### Build y Ejecución Directa:
```bash
# Desde la raíz del proyecto
dotnet build
dotnet run --project src/Presentation
```

## 🧪 Ejecutar Pruebas Unitarias

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar pruebas específicas
dotnet test tests/Application.Tests
dotnet test tests/Domain.Tests
dotnet test tests/Infrastructure.Tests

# Ejecutar con cobertura (necesita coverlet)
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 Funcionalidades del Sistema

### 1. Gestión de Usuarios
- **Crear nuevos usuarios** con validación en tiempo real
- **Editar información** de usuarios existentes
- **Eliminar usuarios** (eliminación lógica)
- **Listar últimos 10 usuarios** creados
- **Validación automática** de campos obligatorios

### 2. Asignación a Departamentos
- **Listar departamentos** disponibles (Nómina, Facturación, IT, etc.)
- **Asignar usuario a departamento** (relación 1:1)
- **Visualizar asignaciones** actuales
- **Eliminar asignaciones** existentes
- **Búsqueda en tiempo real** de usuarios

### 3. Panel de Asignaciones
- **Vista completa** de todas las relaciones usuario-departamento
- **Estadísticas** de usuarios y departamentos
- **Filtrado por búsqueda**
- **Gestión de asignaciones** desde un solo lugar

## 🔧 Configuración Avanzada

### Migraciones de Base de Datos

```bash
# Instalar herramientas de EF Core
dotnet tool install --global dotnet-ef

# Crear nueva migración
dotnet ef migrations add NombreMigracion --project src/Infrastructure --startup-project src/Presentation --output-dir Data/Migrations

# Aplicar migraciones
dotnet ef database update --project src/Infrastructure --startup-project src/Presentation

# Revertir última migración
dotnet ef database update PreviousMigrationName --project src/Infrastructure --startup-project src/Presentation
```

### Cambiar Proveedor de Base de Datos

Para cambiar a otro proveedor (ej: PostgreSQL, MySQL):

1. Instalar el paquete correspondiente:
```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

2. Actualizar `ApplicationDbContext` en `Infrastructure/Data`:
```csharp
options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
```

3. Actualizar `appsettings.json` con la cadena de conexión correspondiente.

## 🐛 Solución de Problemas Comunes

### Error: "A second operation was started..."
**Solución:** Este error ocurre por concurrencia en DbContext. Ya está solucionado usando la propiedad `IsBusy` que serializa las operaciones.

### Error: "No se encuentra la base de datos"
**Solución:**
```bash
# Verificar que LocalDB esté corriendo
sqllocaldb start MSSQLLocalDB

# O crear la base de datos manualmente
dotnet ef database update --project src/Infrastructure --startup-project src/Presentation
```

### Error: "Constructor no encontrado"
**Solución:** Limpiar y reconstruir el proyecto:
```bash
dotnet clean
dotnet build
```

### Botón Guardar no se habilita
**Solución:** Asegurarse de que:
1. Todos los campos obligatorios estén completos
2. El email tenga formato válido (usuario@dominio.com)
3. No haya espacios en blanco al inicio/final

## 📝 Scripts SQL de Creación

Si necesitas recrear la base de datos manualmente, ejecuta:

```sql
-- Encontrarás el script completo en:
-- database/Scripts/CreateDatabase.sql
```

## 🧪 Suite de Pruebas

### Tipos de Pruebas Implementadas:
- **Pruebas unitarias** para comandos y queries
- **Pruebas de integración** con base de datos en memoria
- **Pruebas de dominio** para entidades y reglas de negocio
- **Pruebas de configuración** de Entity Framework

### Cobertura de Pruebas:
- **Application Layer**: Comandos, Queries, Validadores
- **Domain Layer**: Entidades, Value Objects
- **Infrastructure Layer**: DbContext, Configuraciones

## 🔄 Flujo de Trabajo de Desarrollo

### Para agregar nueva funcionalidad:
1. **Crear Command/Query** en `Application/`
2. **Implementar Handler** correspondiente
3. **Actualizar ViewModel** en `Presentation/ViewModels/`
4. **Crear/actualizar Vista** en `Presentation/Views/`
5. **Agregar pruebas unitarias** en `tests/`

### Estructura de un Command:
```csharp
// 1. Definir Command
public class MiNuevoCommand : IRequest<Result>
{
    public string Propiedad { get; set; }
}

// 2. Implementar Handler
public class MiNuevoCommandHandler : IRequestHandler<MiNuevoCommand, Result>
{
    // Implementar lógica
}

// 3. Usar en ViewModel
[RelayCommand]
private async Task EjecutarComando()
{
    var result = await _mediator.Send(new MiNuevoCommand());
}
```

## 📈 Extensión del Proyecto

### Para agregar nuevas entidades:
1. Crear clase en `Domain/Entities/`
2. Agregar DbSet en `ApplicationDbContext`
3. Crear configuración en `Infrastructure/Data/Configurations/`
4. Crear migración: `dotnet ef migrations add AddNuevaEntidad`

### Para agregar nuevas vistas:
1. Crear ViewModel en `Presentation/ViewModels/`
2. Crear View XAML en `Presentation/Views/`
3. Registrar en `App.xaml.cs`
4. Agregar a la navegación principal

## 🤝 Contribuir al Proyecto

1. **Fork** el repositorio
2. **Crear rama** para tu funcionalidad (`git checkout -b feature/nueva-funcionalidad`)
3. **Commit** cambios (`git commit -m 'Agrega nueva funcionalidad'`)
4. **Push** a la rama (`git push origin feature/nueva-funcionalidad`)
5. **Abrir Pull Request**

## 📄 Licencia

Este proyecto fue desarrollado como prueba técnica para AXA Partners Colombia. El código puede ser utilizado como referencia para proyectos similares.

## 📞 Soporte

Para problemas técnicos:
1. Revisar la sección "Solución de Problemas Comunes"
2. Verificar logs de la aplicación (Output en Visual Studio)
3. Ejecutar `dotnet build --verbosity detailed` para ver errores detallados

---

**⚠️ Nota:** Este proyecto requiere .NET 9. Asegúrate de tenerlo instalado antes de intentar ejecutarlo.
