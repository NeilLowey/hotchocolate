using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using StrawberryShake.Generators.Descriptors;
using StrawberryShake.Generators.Utilities;
using static StrawberryShake.Generators.Utilities.NameUtils;

namespace StrawberryShake.Generators
{
    internal class InterfaceModelGenerator
        : SelectionSetModelGenerator<InterfaceType>
    {
        public override ICodeDescriptor Generate(
            IModelGeneratorContext context,
            OperationDefinitionNode operation,
            InterfaceType namedType,
            IType fieldType,
            FieldNode fieldSelection,
            PossibleSelections possibleSelections,
            Path path)
        {
            IFragmentNode returnType = ResolveReturnType(
                context,
                namedType,
                fieldSelection,
                possibleSelections.ReturnType);

            IInterfaceDescriptor interfaceDescriptor = CreateInterfaceModel(
                context, returnType, path);
            context.Register(fieldSelection, interfaceDescriptor);

            CreateClassModels(
                context,
                operation,
                fieldType,
                fieldSelection,
                possibleSelections,
                returnType,
                interfaceDescriptor,
                path);

            return interfaceDescriptor;
        }

        private void CreateClassModels(
            IModelGeneratorContext context,
            OperationDefinitionNode operation,
            IType fieldType,
            FieldNode fieldSelection,
            PossibleSelections possibleSelections,
            IFragmentNode returnType,
            IInterfaceDescriptor interfaceDescriptor,
            Path path)
        {
            var resultParserTypes = new List<ResultParserTypeDescriptor>();
            IReadOnlyCollection<SelectionInfo> selections = possibleSelections.Variants;

            if (selections.Count == 1)
            {
                CreateClassModel(
                    context,
                    returnType,
                    interfaceDescriptor,
                    selections.Single(),
                    resultParserTypes);
            }
            else
            {
                CreateClassModels(
                    context,
                    fieldSelection,
                    returnType,
                    interfaceDescriptor,
                    selections,
                    resultParserTypes,
                    path);
            }

            context.Register(
                new ResultParserMethodDescriptor(
                    GetPathName(path),
                    operation,
                    fieldType,
                    fieldSelection,
                    path,
                    interfaceDescriptor,
                    resultParserTypes));
        }

    }
}
