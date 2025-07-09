# WeatherAPI

Una API de .NET 8 que consume el servicio externo de El Tiempo (`https://www.el-tiempo.net/api/json/v2`) y almacena datos de clima en una base de datos SQL Server.

## 🎯 Características principales

* **Consulta de datos externos**: Provincias, detalles de provincia, municipios y detalle de municipio.
* **Endpoint `/home`**: Obtiene una lista de ciudades con estado del cielo y temperaturas.
* **Persistencia automática**: Hosted Service que actualiza datos desde `/home` al arranque y de forma periódica (configurable).
* **Almacenamiento en SQL Server**: Uso de EF Core con upsert (AddOrUpdate) para mantener un único registro actualizado.
* **Autenticación y autorización**: Usuarios con JWT, registro, login, roles de administrador y cambio de contraseña.
* **Logging centralizado**: NLog para trazas en consola y archivo (`logs/Api.log`).

## 📂 Estructura del proyecto

```text
WeatherAPI/
├─ Weather.api/             # Proyecto Web API (controladores, Program.cs)
├─ Infraestructura/          # Data, Repositories, Services, HostedService, Configuración
├─ Core/                     # DTOs, Entidades, Validadores
└─ README.md                 # Documentación de alto nivel
```

## 🚀 Requisitos previos

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* SQL Server (local o remoto)
* (Opcional) Docker y Docker Compose

## 🔧 Configuración

1. Clona el repositorio:

   ```bash
   ```

git clone [https://github.com/Isra8Rubio/WeatherAPI.git](https://github.com/Isra8Rubio/WeatherAPI.git)
cd WeatherAPI

````

2. Copia y edita `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=WeatherDb;Trusted_Connection=True;"
     },
     "Jwt": {
       "Key": "TU_CLAVE_SECRETA",
       "Issuer": "WeatherAPI",
       "Audience": "WeatherAPIUsers",
       "ExpiresInMinutes": 60
     }
   }
````

3. Asegúrate de que `nlog.config` tenga:

   ```xml
   <rules>
     <logger name="Microsoft.*" minlevel="Info" writeTo="console" />
     <logger name="Infraestructura.Services.*" minlevel="Info" writeTo="file,console" />
     <logger name="*" minlevel="Info" writeTo="file" />
   </rules>
   ```

4. Crea la base de datos y aplica migraciones:

   ```bash
   ```

dotnet ef database update --project Infraestructura --startup-project Weather.api

````

## ▶️ Ejecutar la aplicación

Desde la raíz del repo:
```bash
dotnet run --project Weather.api
````

* Swagger UI disponible en `https://localhost:7131`.
* El Hosted Service se inicia automáticamente y actualiza datos cada 2 minutos.

## ⚙️ Endpoints importantes

* **Provincias**: `GET /api/RemoteWeather/provincias`

* **Detalle provincia**: `GET /api/RemoteWeather/provincias/{cod}`

* **Municipios**: `GET /api/RemoteWeather/provincias/{cod}/municipios`

* **Detalle municipio**: `GET /api/RemoteWeather/provincias/{codProv}/municipios/{codMun}`

* **Home**: `GET /api/RemoteWeather/home`

* **Usuarios**:

  * `POST /api/Users/register`
  * `POST /api/Users/login`
  * `GET /api/Users` (admin)
  * `PUT /api/Users/password`

* **WeatherComplete** (CRUD interno): `GET/POST/PUT/DELETE /api/WeatherComplete`

## 🔄 Hosted Service

La clase `WeatherUpdateHostedService` implementa `IHostedService` y:

1. Llama a `/home` al arrancar y cada intervalo (2 min).
2. Selecciona una ciudad al azar de la lista.
3. Persiste o actualiza la entidad `WeatherComplete` en BD.

Puedes ajustar el intervalo modificando `_interval` en minutos.

## 🧪 Pruebas y desarrollo

* Agrega **unit tests** para los servicios y repositorios.
* Simula respuestas de la API externa con **moq** o **HttpMessageHandler**.

## 📄 Licencia

Este proyecto está bajo la licencia MIT.
