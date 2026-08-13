# SOLIDWORKS integration

This directory contains only SOLIDWORKS-specific components.

- `OpenManage.SolidWorks.Adapter` reads the active SOLIDWORKS document through SOLIDWORKS Interop and converts it to the neutral `EngineeringDocumentInfo` model.
- A future `OpenManage.SolidWorks.AddIn` project will contain commands, UI and add-in registration.
- The console stub in `samples/OpenManage.SolidWorks.ConsoleStub` is a temporary host for integration development.

SOLIDWORKS Interop types must not cross the adapter boundary or be added to `OpenManage.Client`.
