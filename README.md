# vertical-templatestring

Replaces interpolation holes in strings with values from a dictionary, where the template is defined a _runtime_.

## Overview

The `TemplateString` class is a factory that accepts a dictionary of properties, and efficiently replaces tokens in a template to create an output. The template is specified at runtime using "handlebar" notation.

```csharp
using Vertical.Text;

var templateString = TemplateString.Create("{filename}.{format}[.{compression}]");
var properties = new Dictionary<string, object?>
{
    ["filename"] = "output",
    ["format"] = "json",
    ["compression"] = "gz"
};

Console.WriteLine(templateString.Replace(properties));

// Output:
//    output.json.gz

properties["compression"] = null;

Console.WriteLine(templateString.Replace(properties));

// Output:
//    output.json
```

## Configuration notes

- Properties that are always supplied as non-null values can be defined using braced handlebars.
- Properties that the application may supply with `null` values must be defined using bracketed handlebars, otherwise the template will throw an exception during the replacement operation.
- Content inside bracketed handlebars will only be part of replacement strings if a braced handlebar within it has a non-`null` value.
- A bracketed handlebar is ignored as a template if it doesn't contain a braced handlebar.
- Braced handlebars can be ignored using double-brace escaping, e.g. `{{escape}}`.