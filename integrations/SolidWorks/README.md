# SOLIDWORKS integration

This directory contains only SOLIDWORKS-specific components.

- `OpenManage.SolidWorks.Adapter` connects to the running SOLIDWORKS COM application, reads the active document and converts it to the neutral `EngineeringDocumentInfo` model.
- A future `OpenManage.SolidWorks.AddIn` project will contain commands, UI and add-in registration.
- The console stub in `samples/OpenManage.SolidWorks.ConsoleStub` is a temporary host for integration development.

SOLIDWORKS Interop types must not cross the adapter boundary or be added to `OpenManage.Client`. The current adapter uses COM late binding so the solution can build without installing SOLIDWORKS or copying vendor Interop assemblies into the repository.

## Current CreateOnly preparation scenario

The console stub:

1. connects to an already running SOLIDWORKS instance;
2. reads the saved active document;
3. reads document properties and then active-configuration properties;
4. validates that the file is below the workspace root;
5. maps `Обозначение` to attribute `9`, `Наименование` to attribute `10`, and adds relative path attribute `1038`;
6. creates the OpenVault object (`1296` for a part, `1361` for an assembly);
7. adds the mapped attributes;
8. uploads the main file through `POST /api/Storage` with attribute `1002` and link type `4`.

Run on a Windows workstation with SOLIDWORKS open:

```text
OpenManage.SolidWorks.ConsoleStub.exe https://openvault.example/
OpenManage.SolidWorks.ConsoleStub.exe https://openvault.example/ "E:\\AlternativeVault\\"
```

The default workspace is `D:\\Vault\\`. The wrapper keeps `ObjectLinkId` as `long`, but rejects values above `Int32.MaxValue` while the current server Storage contract still uses `int`.
