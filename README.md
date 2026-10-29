# Animal Rescue · DockerDucks

Aplicación académica para registrar rescates animales y consultar una cola ordenada por prioridad. Reutiliza la API de la práctica 2 y agrega una interfaz HTML/CSS/JavaScript integrada en ASP.NET Core .NET 8 para su publicación directa en IIS y Azure App Service, sin Docker ni base de datos.

## Integrantes y responsabilidades

Las contribuciones de esta tabla están **previstas**; deben enlazarse a commits/PR reales cuando cada responsable complete su trabajo. No representan aportes ya verificados.

| Integrante | Responsabilidad | Contribución prevista |
| --- | --- | --- |
| Sara Regino Ferraro | Líder de repositorio | Estructura, acceso de `oalarconpe` y etiqueta final `practica3-v1.0` (plus recomendado). |
| Alejandra Madrid Calderon | IIS | App Pool, sitio, puerto, punto de entrada y notas/archivos de publicación. |
| Paula Andrea Calderon Quintero | Azure | App Service, publicación, Application Setting, HTTPS Only y notas de despliegue. |
| Juan David Velasquez Murillo | Aplicación y `/health` | Interfaz, API, pruebas y preparación de la versión publicable. |
| Jose David Vasquez Diaz | Documentación y video | README final, evidencias organizadas y coordinación/edición del video. |

## Estado de la entrega

| Dato | Estado |
| --- | --- |
| Aplicación y publicación Release | Verificadas localmente en Linux; falta validación en los destinos reales. |
| Fecha de publicación en IIS | Pendiente. |
| Fecha de publicación en Azure | Pendiente. |
| URL pública HTTPS de Azure | Pendiente. |
| Video con participación de todos | Pendiente. |
| Acceso del docente `oalarconpe` | Pendiente de verificar en GitHub, usando el nombre de usuario, no un correo. |

## Ejecutar localmente

Requiere **SDK .NET 8**. Desde la raíz del repositorio:

```sh
dotnet run --project src/DockerDucks.Api --no-launch-profile --urls http://localhost:5080
```

Abrir `http://localhost:5080/`. Recursos disponibles:

| Ruta | Función |
| --- | --- |
| `/` | Formulario y cola de atención. |
| `/health` | Salud básica del proceso; debe responder HTTP 200 con `Healthy`. |
| `/swagger/` | Exploración y pruebas manuales de la API. |
| `POST /api/rescue-reports` | Registrar un reporte. |
| `GET /api/rescue-reports/queue` | Cola por prioridad y antigüedad. |
| `GET /api/rescue-reports/{id}` | Consultar un reporte por identificador. |

La prioridad la calcula el servidor: condición crítica o peligro inmediato → **crítica**; herido → **alta**; estable → **media**. A igual prioridad se atiende primero el reporte más antiguo. La cola se consulta al abrir la página, después de registrar y con **Actualizar**; no hay actualización en tiempo real.

> Los datos existen solo en memoria: se pierden al reiniciar, volver a desplegar o reciclar el proceso. Cada instancia tiene su propia cola. Usar una sola instancia/proceso para la demostración. No hay autenticación: cualquiera con acceso puede leer y crear reportes. No ingresar información sensible. Esta aplicación no contacta servicios de emergencia.

## Pruebas

API, prioridades e interfaz servida en Production:

```sh
dotnet test DockerDucks.sln --configuration Release
```

Pruebas de navegador opcionales (requieren Node.js 20+; **Node no se necesita en producción**):

```sh
cd tests/browser
npm ci
npx playwright install chromium
npm test
```

La suite inicia una aplicación local dedicada en `http://127.0.0.1:5187`, registra datos de prueba y la detiene al terminar. En Linux puede ser necesario instalar las dependencias de sistema de Chromium siguiendo el diagnóstico de Playwright. No ejecutar contra una entrega compartida: crea reportes.

Para probar un artefacto publicado que ya esté ejecutándose localmente, definir `BASE_URL` antes de `npm test`: en PowerShell, `$env:BASE_URL="http://127.0.0.1:5188"`; en bash, `BASE_URL=http://127.0.0.1:5188 npm test`.

## Preparar la publicación para IIS y Azure

Desde la raíz del repositorio, después de aprobar las pruebas:

```sh
dotnet publish src/DockerDucks.Api/DockerDucks.Api.csproj --configuration Release --output artifacts/publish
```

Publicar **todo el contenido de `artifacts/publish/`**, no el código fuente ni solo el ejecutable. Incluye `DockerDucks.Api.dll`, dependencias, `wwwroot/` y el `web.config` generado automáticamente para ASP.NET Core Module. No editarlo para convertir la app en un sitio puramente estático ni eliminarlo.

| Destino | Preparación y verificación pendientes del responsable |
| --- | --- |
| IIS en Windows 11 | Habilitar IIS; instalar/reparar .NET 8 Hosting Bundle después de IIS; crear App Pool dedicado (`No Managed Code` recomendado), sitio y binding en un puerto disponible; conceder lectura/ejecución de la carpeta publicada a la identidad del App Pool. Verificar `/`, registro/cola y `/health`. |
| Azure App Service | Flujo propuesto: Windows, publicación de código, runtime .NET 8 disponible y una instancia. Confirmar plan/costo antes de crear recursos. Configurar `ASPNETCORE_ENVIRONMENT=Production` y HTTPS Only. Publicar la misma versión validada en IIS y verificar interfaz, API y `/health` desde la URL pública HTTPS. |

El puerto de acceso de IIS se configura en el **binding**, no copiando variables de Docker o perfiles de IIS Express. Las rutas del frontend son relativas y no contienen hostnames de desarrollo. Los reportes no se comparten entre IIS y Azure: son dos procesos independientes.

Si se utiliza ZIP Deploy en Azure, comprimir el **contenido** de la carpeta de publicación, sin agregar una carpeta contenedora adicional. No incluir credenciales, perfiles privados de publicación ni cadenas de conexión en GitHub o en las capturas.

## Evidencias y cierre

- `evidencias/iis/`: App Pool, sitio/ruta física/binding, interfaz funcionando y `/health`.
- `evidencias/azure/`: App Service, Application Settings sin secretos, HTTPS Only, interfaz pública y `/health`.
- Completar fechas, URL y video en este README solo después de verificarlos.
- Cada integrante debe hacer una contribución significativa verificable y participar verbalmente en el video.
- Confirmar acceso efectivo de `oalarconpe` y probar los enlaces desde otra sesión.

Las capturas locales de desarrollo no sustituyen evidencias de IIS o Azure. Esta versión prepara el despliegue; no afirma que esos entornos ya estén publicados.

## Referencias

- Base: [Practica2Contenerizacion_DockerDucks](https://github.com/ArsaOniSaturn/Practica2Contenerizacion_DockerDucks), reutilizada desde la copia local del equipo; commit de origen `43c83be41d0004358b93574fe7b06a24c0c9e70e`. La disponibilidad del enlace remoto debe verificarse antes de la entrega.
- [ASP.NET Core en IIS — Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-8.0).
- [Publicación ZIP en App Service — Microsoft Learn](https://learn.microsoft.com/en-us/azure/app-service/deploy-zip).
