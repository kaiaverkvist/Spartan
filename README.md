# Spartan

Web framework written in C#

[![buddy pipeline](https://app.buddy.works/kaiaverkvist/spartan/pipelines/pipeline/202904/badge.svg?token=1f859e3912a306e35092860dd08641ff372682b3717cb8c4f8d6f2e484972411 "buddy pipeline")](https://app.buddy.works/kaiaverkvist/spartan/pipelines/pipeline/202904)

# Vision
I wanted to create a lightweight MVC framework written in C#. The goal is to implement a templating engine, routing, cookies and other common patterns from various modern MVC frameworks.

# Contribution
Contributions are welcome. I am following ReSharper style guidelines for the code.

# Usage
Using the Library should be pretty simple.

Initializing the server:

* The server creates a directory named `_Public` in the execution directory. Files in this directory are served by calling the path `/public/file.txt` in your HTML.

## Code examples

```cs
static void Main(string[] args)
{
    SpartanConfiguration config = new SpartanConfiguration();
    SpartanServer server = new SpartanServer(config);

    // Start the server.
    server.Run();

    Console.WriteLine("Running :)");

    // Wait for a readkey so it stays running.
    Console.ReadKey();

    // Stop it after the readkey.
    server.Stop();
}
```

Once initialized, you can define routes using basic classes with attributes.
Your view classes can have any name, but must inherit from IView, and have the Route attribute in order to be discovered.
The ImageView class is in this case instantiated once the server starts, and each time a request hits `localhost/`, the server calls GetView() with the context.
The return statement is what gets sent to the client as a string.

```cs
[Route("/")]
public class ImageView : IView
{
    [Replicate]
    public string Image;

    [Replicate]
    public string User;

    [Replicate]
    public string Time;

    public string GetView(HttpListenerContext context, TemplatingProcessor templatingProcessor)
    {
        FileInfo file = new FileInfo("_Templates/image.html");

        Image = "/public/fahrenheit-451.png";
        User = context.Request.UserAgent;
        Time = DateTime.Now.ToUniversalTime().ToString();

        // Define a new template using the data we just set up.
        // We're also passing in *this* as the last parameter so our reflection engine can find replicatable attributes.
        SpartanTemplate template = new SpartanTemplate(file, templatingProcessor, this);

        // Render the template.
        return template.Render();
    }
}
```

and in your actual template file (in the _Templates folder):
```html
<h1>Test image:</h1>
<p>This is rendered from the template file image.html</p>
<img src="{{ Image }}" width="300px">
<p>Your useragent: {{ User }}</p>
<p>Current time: {{ Time }}</p>
{{ Image }}
```

Result from the above code (Assuming you have an image in the _Public folder):

![result](https://i.imgur.com/cM2KGMx.png)
