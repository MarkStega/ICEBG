---
uid: A.Installation
title: Installation
---
# Installation

Either fork this repo or reference the Materia.Blazor Nuget package. Once the package is referenced in your project you will need to add the CSS and JS in your html.

Add the `Materia.Blazor` namespace to your project by appending `@using Materia.Blazor` to the end of your project's _Imports.razor file. Do not use components from the `Materia.Blazor.Internal` namespace: as the name
implies these are intended for internal use by Materia.Blazor, however Blazor has no mechanism for internally restricted Blazor components to mirror the `internal` directive for a C# class.

## Obtaining the requisite CSS and JS

You will to add three items to your index.html/_Layout.cshtml. Place this in the `<head>` tag:

```html
<link href="_content/Materia.Blazor/material-components-web.min.css" rel="stylesheet" />
<link href="_content/Materia.Blazor/Materia.Blazor.min.css" rel="stylesheet" />
```

and at the end of `<body>`:

```html
<script src="_content/Materia.Blazor/Materia.Blazor.min.js"></script>
```

 Replace the material-components-web.min.css with your own
if you have built a theme - you can see how we have done this in the [Materia.Blazor website's index.html](https://github.com/Material-Blazor/Materia.Blazor/blob/main/Materia.Blazor.Website..WebAssembly/wwwroot/index.html#L14).

Materia.Blazor.min.css includes the Material icons for convenience. If you wish to use either (or both) Font Awesome or Open Iconic icon sets see the next section for the additional css required.

The non-minified versions of each of the css and js packages are also available if needed for debugging.

All styling really should be done with Material but the application created with the templates uses Bootstrap. Once you have your apllication fully in Material remove the reference to bootstrap.min.css.


## Package versions

Materia.Blazor works with the following package versions:

- [Material Components v14.0.0](https://github.com/material-components/material-components-web/blob/master/CHANGELOG.md#1200-2021-07-27)

## Services and Anchor

Materia.Blazor has three services for logging, snackbars, and toasts. We strongly advise you to use these in your project.

```csharp
services.AddMBServices();
```

The three configurations are either the default (as above) or custom - 
see [MBSnackbarServiceConfiguration](xref:Materia.Blazor.MBSnackbarServiceConfiguration),
[MBToastServiceConfiguration](xref:Materia.Blazor.MBToastServiceConfiguration),
and [MBLoggingServiceConfiguration](xref:Materia.Blazor.MBLoggingServiceConfiguration).

When you use the services you must also place an anchor component at the top of `App.razor` - this must not be inside any other components or divs:

```html
<MBAnchor />
```

## Binding

Materia.Blazor components support the EditForm environment. To that end Materia.Blazor uses the 

```csharp
FieldIdentifier.Create(ValueExpression) 
```

construct. This means that values to be bound are limited to fields and properties. As an example, should you try to bind to an array element as in

```html
<Component @bind-Value="@boolArray[0]" />
```

you will be met with a runtime error of

`Error: System.ArgumentException: The provided expression contains a SimpleBinaryExpression which is not supported. FieldIdentifier only supports simple member accessors (fields, properties) of an object.`

