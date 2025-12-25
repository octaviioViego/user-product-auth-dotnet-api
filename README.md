# Información del proyecto

Este repositorio contiene prácticas y ejemplos para aprender y desarrollar APIs REST con .NET.  
Incluye ejercicios de:

- Configuración del entorno
- Creación de modelos y entidades
- Autenticación y manejo de tokens
- Serialización de datos y manejo de vistas
- Pruebas y validaciones
- Buenas prácticas de diseño aplicando los principios SOLID

Este proyecto es ideal para quienes desean profundizar en el desarrollo backend con .NET y mejorar sus habilidades en arquitectura y diseño de software.

# Tecnologías utilizadas
- **.NET 7**
- **C#**
- **PostgreSQL**
- **Swagger / OpenAPI**
- **Entity Framework Core** (ORM)
- **JWT** (Autenticación)
- **Git / GitHub** (Control de versiones)

# Requisitos Previos:
Antes de ejecutar el proyecto, es necesario instalar **.NET**. Puedes descargarlo e instalarlo desde el [sitio web oficial](https://dotnet.microsoft.com/es-es/download). Ademas de instalar Postgrest en tu computadora.

### Pasos para Ejecutar el Proyecto:
1. **Abrir la Terminal**:
   - Abre una terminal o línea de comandos en tu computadora.
  
2. **Navegar al Directorio del Proyecto**:
   - Utiliza el comando `cd` para cambiar al directorio donde se encuentra tu proyecto. Debes estar en el mismo directorio que el archivo `Program.cs`.
   - Ejemplo: Si tu proyecto está en el escritorio, podrías escribir:
     ```bash
     cd Desktop/BackEnd-Net
     ```
3. **Ejecutar la migracion de BD**:
    - Utiliza el comando `dotnet ef migrations add PrimeraMigracionUsuarios --context DataContex` para preparar la primera migracion. 

    - Utiliza el comando `dotnet ef migrations add PrimeraMigracionProductos --context DataContextProduct` para preparar la segunda migracion. 
    
    - Despues ejecuta las migraciones con los comandos
      `dotnet ef database update --context DataContext` para crear la tabla Users.
      `dotnet ef database update --context DataContextProduct` para crear la tabla. Products.

4. **Ejecutar Proyecto**:
    - Finalmente ejecuta el programa `Program.cs` `con el siguiente comando:
        ```bash
        dotnet run
        ```
    - Este comando iniciará el proyecto
  
5. **Disfruta**.

# Información extra
- **Autor:** Fernando Octavio Arroyo Velasco
- **Formación:** Estudiante de Ingeniería de Software
- **Universidad:** Universidad Autónoma de la Ciudad de México (UACM)
