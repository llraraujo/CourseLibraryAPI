using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CourseLibrary.API.Helpers
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // This checks if our model is enumerable. Our binder works only on enumerable types
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            //Get the inputted value through the value provider
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

            //If that value is null or whitespace, we return null
            if (string.IsNullOrWhiteSpace(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            //Get the enumerable's Type and a converter
            var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0]; // Type
            var converter = TypeDescriptor.GetConverter(elementType);  // Converter

            //Convert each item in the value list to the enumerable type
            var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(element => converter.ConvertFromString(element.Trim()))
                                    .ToArray();

            // Create an array of that type and set it ad the Model Value
            var typedValues = Array.CreateInstance(elementType, values.Length);

            // Copy the values array to the typedValues Array
            values.CopyTo(typedValues, 0);

            bindingContext.Model = typedValues;

            // return a successful result, passing in the Model
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;

                            
            
        }
    }
}
