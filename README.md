# BlazorCocktails 🍸

Aplicación **full-stack** en **.NET 8** para explorar y gestionar cócteles.  
Incluye **API REST** (JWT + FluentValidation + EF Core) y **cliente Blazor WebAssembly** con MudBlazor, localización ES/EN, tema oscuro y métricas visuales.

---

## ✨ Características

- **Autenticación JWT**
  - Registro, login y logout.
  - Persistencia del token (local/session storage).
  - Hook en el cliente NSwag para enviar `Authorization: Bearer …`.
  - Páginas protegidas y chip **Admin** en la AppBar para el rol.

- **UI moderna (MudBlazor)**
  - Tema **oscuro forzado**, gradientes suaves y diseño responsive.
  - Páginas de **Login** y **Registro** pulidas (toggle de contraseña, validaciones UX).
  - **Home** con “hero”.

- **Localización**
  - Interfaz en **español** e **inglés** mediante `.resx`.
  - Conmutador de idioma en la AppBar.

- **Dominio / navegación**
  - Listados por **Alcohol**, **Categorías** y **Vasos**.
  - **Detalle** de cóctel.
  - Menú lateral visible solo con sesión iniciada.

- **Calidad y DX**
  - **FluentValidation** en la API.
  - **NSwag** para cliente tipado (`ApiClient`).
  - Snackbars y mensajes legibles para errores de API.

---

## 🧱 Estructura

~~~text
BlazorCocktails/
├─ ApiClient/                 # Cliente NSwag (parciales con hook del token)
├─ BlazorCocktails.Client/    # Blazor WebAssembly (UI, páginas, servicios)
├─ Cocktail.api/              # API REST (.NET 8, JWT, FluentValidation)
├─ Core/                      # Entidades, DTOs, contratos
├─ Infraestructura/           # EF Core, repositorios, migraciones
└─ README.md
~~~

---

## 🚀 Requisitos

- **.NET 8 SDK**
- **SQL Server** (LocalDB o instancia)
- (Opcional) **EF Core Tools**

~~~bash
dotnet tool install --global dotnet-ef
~~~

---

## 🔧 Configuración rápida

### 1) Clonar

~~~bash
git clone https://github.com/Isra8Rubio/BlazorCocktails.git
cd BlazorCocktails
~~~

### 2) API – `Cocktail.api/appsettings.json`

Ajusta conexión y JWT:

~~~json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=CocktailsDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "TU_CLAVE_SECRETA_LARGA_Y_SEGURA",
    "Issuer": "CocktailApi",
    "Audience": "CocktailClient",
    "ExpiresInMinutes": 60
  }
}
~~~

### 3) Migraciones / Base de datos

~~~bash
dotnet ef database update --project Infraestructura --startup-project Cocktail.api
~~~

### 4) Ejecutar API

~~~bash
dotnet run --project Cocktail.api
~~~

> Swagger estará disponible al iniciar la API (URL de desarrollo).

### 5) Ejecutar Cliente (Blazor WASM)

~~~bash
dotnet run --project BlazorCocktails.Client
~~~

> Abre la URL que muestre la consola (por ejemplo, `https://localhost:****`).

---

## 🔐 Autenticación y token

- El **login** devuelve un JWT que el cliente guarda en:
  - `localStorage` si marcas **Recordarme**,
  - `sessionStorage` si no lo marcas.
- `ApiClient` (NSwag) incluye un **partial** que añade `Authorization` en cada request.
- `App.razor` **restaura** el token antes de renderizar el Router (evita `401` al recargar/cambiar idioma).
- `MainLayout` **reacciona** a cambios del token (Drawer, botones Login/Logout, chip Admin).

---

## 🌍 Localización

- Archivos `.resx` en el cliente (**es-ES** / **en-US**).
- Claves agrupadas: `Login_*`, `Register_*`, `Home_*` para localizarlas rápido.
- Selector de idioma en la AppBar.

---

## 📊 Gráficos & Widgets

- `AlcoholTypesChart` y `CategoryDistributionChart` (MudBlazor Charts).
- `RandomCocktailWidget` para propuesta instantánea.

---

## 🧪 Desarrollo

- **Regenerar cliente NSwag** si cambias la API.  
  (El partial `APIClient.Partials.cs` mantiene el hook del token).
- **Validaciones**: reglas con FluentValidation en la API + validaciones UX ligeras en formularios.
- **Tema**: modo **oscuro** fijo (conmutador deshabilitado por diseño).

---

## ⚠️ Problemas comunes

**Migraciones**  
Verifica la cadena de conexión y ejecuta:

~~~bash
dotnet ef database update --project Infraestructura --startup-project Cocktail.api
~~~

---

## 🗺️ Roadmap

- Recuperación de contraseña.
- Buscador avanzado.
- Más métricas (vasos más usados, top categorías por periodo).
- *Seed* de datos y creación de admin desde consola.

---

## 📄 Licencia

Distribuido bajo **licencia MIT**.
