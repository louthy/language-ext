using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace LanguageExtAnalysers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class CollectionInitialisationAnalyser : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "LANGEXT001";

        private const string Description = "Collection initialisers call the Add method but ignore the return value, which is incorrect behaviour for immutable types";

        private const string Title = "Immutable language-ext type created with collection initialiser";
        public const string MessageFormat = "Don't use collection initialisers with immutable language-ext types";
        private const string Category = "Design";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyseCollectionInitialiserNode, SyntaxKind.CollectionInitializerExpression);
        }

        private static void AnalyseCollectionInitialiserNode(SyntaxNodeAnalysisContext context)
        {
            // Do as many early-exit checks as possible on the already-available SyntaxNode before accessing the SemanticModel (which is more expensive)
            if ((context.Node is not InitializerExpressionSyntax initialiser) || !initialiser.Expressions.Any() || (initialiser.Parent is not ObjectCreationExpressionSyntax objectCreation))
                return;

            // Note: Check "context.SemanticModel.GetTypeInfo(objectCreation).ConvertedType" and not "context.SemanticModel.GetSymbolInfo(objectCreation.Type).Symbol" because if the object creation
            // is of the form "List<int> list4 = new() { 4, 5, 6 };" then the objectCreation.Type value will be reported as a TupeType, rather than the actual type being constructed
            if ((context.SemanticModel.GetTypeInfo(objectCreation).ConvertedType is not ITypeSymbol typeBeingCreated) || (typeBeingCreated.Kind == SymbolKind.ErrorType))
                return;

            // This analyser should only target the types in the LanguageExt library
            if (typeBeingCreated.ContainingAssembly.Identity.Name != "LanguageExt.Core")
                return;

            // We know that there is an "Add" method available, otherwise the code wouldn't compile. It would be more precise to find the Add method that is called by the collection initialiser
            // but this approach should suffice for the purposes here.
            var allAddMethodsReturnInitialType = typeBeingCreated
                .GetMembers(WellKnownMemberNames.CollectionInitializerAddMethodName)
                .OfType<IMethodSymbol>()
                .All(m => m.ReturnType.Equals(typeBeingCreated, SymbolEqualityComparer.Default));

            if (!allAddMethodsReturnInitialType)
            {
                // If there is an Add method that doesn't return the current type then it doesn't follow the expected pattern and we won't raise any diagnostic messages for it
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }
    }
}
