# Comidify Backend - Guía de Instalación y Configuración

## 📋 Tabla de Contenidos
1. [Requisitos Previos](#requisitos-previos)
2. [Configuración de Supabase](#configuración-de-supabase)
3. [Configuración del Proyecto](#configuración-del-proyecto)
4. [Ejecutar Migraciones](#ejecutar-migraciones)
5. [Ejecutar el Proyecto](#ejecutar-el-proyecto)
6. [Despliegue en Render](#despliegue-en-render)
7. [Endpoints del API](#endpoints-del-api)

## 🔧 Requisitos Previos

### 1. Instalar .NET 8 SDK
1. Ve a https://dotnet.microsoft.com/download/dotnet/8.0
2. Descarga el instalador para tu sistema operativo
3. Ejecuta el instalador
4. Verifica la instalación abriendo una terminal:
   ```bash
   dotnet --version
   ```
   Deberías ver algo como `8.0.x`

### 2. Instalar un Editor de Código
**Opción A: Visual Studio Code (Recomendado para comenzar)**
1. Descarga de https://code.visualstudio.com/
2. Instala la extensión "C#" de Microsoft

**Opción B: Visual Studio Community (Más completo)**
1. Descarga de https://visualstudio.microsoft.com/
2. Durante la instalación, selecciona "ASP.NET and web development"

### 3. Instalar Git
1. Descarga de https://git-scm.com/downloads
2. Instala con las opciones por defecto

## 🗄️ Configuración de Supabase

### Paso 1: Crear un Nuevo Proyecto en Supabase

1. Ve a https://supabase.com/ e inicia sesión
2. Click en "New Project"
3. Completa la información:
   - **Name**: `comidify`
   - **Database Password**: Crea una contraseña segura (¡Guárdala!)
   - **Region**: Selecciona la más cercana (ej: `South America (São Paulo)`)
   - **Pricing Plan**: Free
4. Click en "Create new project"
5. Espera 2-3 minutos mientras se crea el proyecto

### Paso 2: Obtener la Cadena de Conexión

1. En tu proyecto de Supabase, ve a **Settings** (⚙️) en el menú lateral
2. Click en **Database**
3. Scroll hacia abajo hasta la sección **Connection string**
4. Selecciona la pestaña **URI**
5. Copia la cadena de conexión. Se verá así:
   ```
   postgresql://postgres.xxxxxxxxxxxxx:[YOUR-PASSWORD]@aws-0-us-east-1.pooler.supabase.com:6543/postgres
   ```
6. Reemplaza `[YOUR-PASSWORD]` con la contraseña que creaste en el Paso 1

**Ejemplo de cadena de conexión completa:**
```
postgresql://postgres.abcdefghijklmnop:MiPassword123@aws-0-us-east-1.pooler.supabase.com:6543/postgres
```

## ⚙️ Configuración del Proyecto

### Paso 1: Clonar o Descargar el Código

Si tienes el código en un repositorio:
```bash
git clone [URL_DEL_REPOSITORIO]
cd Comidify/Comidify.Backend
```

Si estás empezando desde cero, los archivos ya están creados en la carpeta `Comidify.Backend`.

### Paso 2: Configurar la Conexión a la Base de Datos

1. Abre el archivo `appsettings.json`
2. Reemplaza la cadena de conexión con la que obtuviste de Supabase:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "postgresql://postgres.xxxxx:[TU-PASSWORD]@aws-0-us-east-1.pooler.supabase.com:6543/postgres"
  }
}
```

**⚠️ IMPORTANTE**: No subas este archivo a un repositorio público. El `.gitignore` ya está configurado para proteger archivos sensibles.

### Paso 3: Restaurar Paquetes

Abre una terminal en la carpeta `Comidify.Backend` y ejecuta:

```bash
dotnet restore
```

Esto descargará todos los paquetes NuGet necesarios.

## 🔄 Ejecutar Migraciones

Las migraciones crean las tablas en la base de datos según nuestros modelos.

### Paso 1: Instalar la Herramienta de Entity Framework (si no la tienes)

```bash
dotnet tool install --global dotnet-ef
```

Si ya la tienes instalada, actualízala:
```bash
dotnet tool update --global dotnet-ef
```

### Paso 2: Crear la Migración Inicial

```bash
dotnet ef migrations add InitialCreate
```

Este comando creará una carpeta `Migrations` con los archivos de migración.

### Paso 3: Aplicar la Migración a la Base de Datos

```bash
dotnet ef database update
```

Este comando ejecutará las migraciones y creará todas las tablas en Supabase.

### Verificar en Supabase

1. Ve a tu proyecto en Supabase
2. Click en **Table Editor** en el menú lateral
3. Deberías ver las siguientes tablas:
   - Comidas
   - Ingredientes
   - ComidaIngredientes
   - MenusSemanales
   - MenuComidas

## 🚀 Ejecutar el Proyecto

### En Modo Desarrollo

```bash
dotnet run
```

O con hot reload (recarga automática al hacer cambios):
```bash
dotnet watch run
```

El API estará disponible en:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger UI**: https://localhost:5001/swagger

### Probar el API

Abre tu navegador y ve a `https://localhost:5001/swagger` para ver la documentación interactiva del API.

## 📤 Despliegue en Render

### Paso 1: Preparar el Repositorio

1. Crea un repositorio en GitHub
2. Sube tu código:
   ```bash
   git init
   git add .
   git commit -m "Initial commit"
   git branch -M main
   git remote add origin [URL_DE_TU_REPO]
   git push -u origin main
   ```

### Paso 2: Crear un Web Service en Render

1. Ve a https://render.com/ e inicia sesión
2. Click en **New +** → **Web Service**
3. Conecta tu repositorio de GitHub
4. Configura el servicio:
   - **Name**: `comidify-api`
   - **Region**: Selecciona la más cercana
   - **Branch**: `main`
   - **Root Directory**: `Comidify.Backend` (si tu estructura lo requiere)
   - **Runtime**: `.NET`
   - **Build Command**: `dotnet publish -c Release -o out`
   - **Start Command**: `cd out && dotnet Comidify.API.dll`

### Paso 3: Configurar Variables de Entorno

En Render, ve a **Environment**:

1. Click en **Add Environment Variable**
2. Agrega:
   - **Key**: `ConnectionStrings__DefaultConnection`
   - **Value**: Tu cadena de conexión de Supabase completa
3. Click en **Save Changes**

**Nota**: En variables de entorno, los dos puntos (`:`) se reemplazan por doble guion bajo (`__`).

### Paso 4: Desplegar

1. Click en **Create Web Service**
2. Render automáticamente construirá y desplegará tu aplicación
3. Una vez completado, obtendrás una URL como: `https://comidify-api.onrender.com`

### Paso 5: Ejecutar Migraciones en Producción

Opción 1: Desde tu máquina local, actualiza `appsettings.json` temporalmente con la cadena de producción y ejecuta:
```bash
dotnet ef database update
```

Opción 2: Usa el shell de Render para ejecutar las migraciones directamente en el servidor.

## 📚 Endpoints del API

### Comidas
- `GET /api/comidas` - Obtener todas las comidas (con filtros opcionales)
  - Query params: `nombre`, `tipoComida`
- `GET /api/comidas/{id}` - Obtener una comida específica
- `POST /api/comidas` - Crear una nueva comida
- `PUT /api/comidas/{id}` - Actualizar una comida
- `DELETE /api/comidas/{id}` - Eliminar una comida

### Ingredientes
- `GET /api/ingredientes` - Obtener todos los ingredientes
  - Query params: `nombre`
- `GET /api/ingredientes/{id}` - Obtener un ingrediente específico
- `POST /api/ingredientes` - Crear un nuevo ingrediente
- `PUT /api/ingredientes/{id}` - Actualizar un ingrediente
- `DELETE /api/ingredientes/{id}` - Eliminar un ingrediente

### Menús Semanales
- `GET /api/menussemanales` - Obtener todos los menús
- `GET /api/menussemanales/{id}` - Obtener un menú específico
- `POST /api/menussemanales` - Crear un nuevo menú
- `PUT /api/menussemanales/{id}` - Actualizar un menú
- `DELETE /api/menussemanales/{id}` - Eliminar un menú
- `GET /api/menussemanales/{id}/lista-compras` - Obtener lista de compras

## 🧪 Probar el API

### Ejemplo: Crear un Ingrediente

```bash
curl -X POST https://localhost:5001/api/ingredientes \
  -H "Content-Type: application/json" \
  -d '{"nombre": "Huevo"}'
```

### Ejemplo: Crear una Comida

```bash
curl -X POST https://localhost:5001/api/comidas \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Huevos Revueltos",
    "tipoComida": 1,
    "ingredientes": [
      {
        "ingredienteId": 1,
        "cantidad": "2",
        "unidad": "piezas"
      }
    ]
  }'
```

## 📝 Tipos de Comida (Enum)

```
1 = Desayuno
2 = Almuerzo
3 = Comida
4 = Merienda
5 = Cena
```

## 📅 Días de la Semana (Enum)

```
1 = Lunes
2 = Martes
3 = Miercoles
4 = Jueves
5 = Viernes
6 = Sabado
7 = Domingo
```

## 🐛 Solución de Problemas

### Error: "Unable to connect to the database"
- Verifica que la cadena de conexión en `appsettings.json` sea correcta
- Asegúrate de que tu IP esté permitida en Supabase (por defecto permite todas)

### Error: "The name 'DbContext' does not exist"
- Ejecuta `dotnet restore` para restaurar los paquetes

### Error al ejecutar migraciones
- Verifica que tengas instalado `dotnet-ef`: `dotnet tool install --global dotnet-ef`
- Asegúrate de estar en la carpeta correcta donde está el archivo `.csproj`

### El API no responde en Render
- Revisa los logs en el dashboard de Render
- Verifica que las variables de entorno estén configuradas correctamente
- Asegúrate de que las migraciones se hayan ejecutado en la base de datos de producción

## 🎉 Próximos Pasos

Una vez que el backend esté funcionando:
1. ✅ Probar todos los endpoints en Swagger
2. 📱 Construir el frontend en React
3. 🔗 Conectar el frontend con el backend
4. 🚀 Desplegar ambos en producción

## 📞 Soporte

Si tienes problemas, revisa:
1. Los logs de la consola donde ejecutas `dotnet run`
2. Los logs en Render (si estás desplegando)
3. La documentación de Swagger en `/swagger`
