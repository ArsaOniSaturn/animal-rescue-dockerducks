# Evidencias CI/CD — pendientes de ejecución remota

Preparación local no equivale a despliegue Azure Pipelines verificado.
Las imágenes de `../azure/` y `../iis/` pertenecen a la entrega histórica.
Guía: [configuración CI/CD](../../docs/CI_CD.md).

## Checklist por completar con evidencia real

- [ ] SHA de main y contribuciones reales de cada integrante, sin atribuir
  contribuciones futuras usando la tabla histórica.
- [ ] URL del run Azure DevOps y BuildId; trigger automático tras cambio main.
- [ ] CI Windows/.NET 8: restore, build Release y resultado de tests.
- [ ] Artefacto `animal-rescue-web`, ZIP `animal-rescue.zip` descargable;
  DLL, web.config, wwwroot y build-info.json sin carpeta contenedora.
- [ ] CD/environment `animal-rescue-azure`: éxito y descarga del mismo run,
  sin checkout/rebuild; AzureWebApp Windows zipDeploy.
- [ ] URL pública real, `/health`, `/` y JSON con SHA/BuildId del mismo run;
  captura de logs del smoke exitoso, no solo página pública histórica.
- [ ] Video 1: enlace accesible y participación exigida por la rúbrica.
- [ ] Video 2: enlace accesible y participación exigida por la rúbrica.
- [ ] PDF final con enlaces, resultados y evidencias exigidas por la rúbrica.

Los guiones de ambos videos se prepararán **después de verificar** la
implementación remota. No hay enlaces ficticios ni evidencia de CD en esta
carpeta. Revisar accesibilidad para el docente y ocultar credenciales,
identificadores personales innecesarios, tokens y pantallas de autenticación.

Validación local ya realizada: 17 tests Release aprobados, publicación
framework-dependent, estructura ZIP y smoke del paquete extraído con identidad
local. Ver resultados y límites en la guía; no acreditan CD Azure.
