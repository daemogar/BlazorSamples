using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Diagnostics;
using System.Text;

namespace SampleSourceGenerators
{
	[Generator]
    public class HelloWorldGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            // begin creating the source we'll inject into the users compilation
            var sourceBuilder = new StringBuilder(@"
using System;
using System.Diagnostics;

namespace HelloWorldGenerated
{
    public static class HelloWorld
    {
        public static void SayHello() 
        {
            Debug.WriteLine(""Hello from generated code!"");
            Debug.WriteLine(""The following syntax trees existed in the compilation that created this program:"");
");

            // using the context, get a list of syntax trees in the users compilation
            var syntaxTrees = context.Compilation.SyntaxTrees;

            sourceBuilder.AppendLine($@"            Debug.WriteLine(@""==============================="");");
            sourceBuilder.AppendLine($@"            Debug.WriteLine(@""Additional Files"");");
            foreach (var addtionalFile in context.AdditionalFiles)
			{
                sourceBuilder.AppendLine($@"            Debug.WriteLine(@"" - {addtionalFile.Path}"");");
            }

            sourceBuilder.AppendLine($@"            Debug.WriteLine(@""==============================="");");
            sourceBuilder.AppendLine($@"            Debug.WriteLine(@""Syntax Trees"");");
            // add the filepath of each tree to the class we're building
            foreach (SyntaxTree tree in syntaxTrees)
            {
                sourceBuilder.AppendLine($@"            Debug.WriteLine(@"" - {tree.FilePath}"");");
            }

            // finish creating the source to inject
            sourceBuilder.Append(@"
        }
    }
}");

            // inject the created source into the users compilation
            context.AddSource("helloWorldGenerator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}
