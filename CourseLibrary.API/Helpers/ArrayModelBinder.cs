using AutoMapper.Execution;
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
    public Task BindModelAsync(ModelBindingContext bindingContext) {
      //The binder only works on enumerable types
      if(!bindingContext.ModelMetadata.IsEnumerableType) {
        bindingContext.Result = ModelBindingResult.Failed();
        return Task.CompletedTask;
      }

      //Get the inputed value through the value provider
      var value = bindingContext.ValueProvider.
        GetValue(bindingContext.ModelName).ToString();

      //If that value is null or whitespace we return null
      if (string.IsNullOrEmpty(value)) {
        bindingContext.Result = ModelBindingResult.Success(null);
        return Task.CompletedTask;
      }

      //The value isn't null or whitespace
      //and the type of the model is enumerable
      //Get the enumerable's type, and a converter
      var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
      var converter = TypeDescriptor.GetConverter(elementType); //This is the converter to go from string to Guid

      //Convert each item in the value list to the enumerable type
      var values = value.Split(new [] { "," }, StringSplitOptions.RemoveEmptyEntries)
        .Select(x => converter.ConvertFromString(x.Trim()))
        .ToArray();

      //Create an array of that type and set it as the Model value
      var typedValues = Array.CreateInstance(elementType, values.Length);
      values.CopyTo(typedValues, 0);
      bindingContext.Model = typedValues;

      //return a successful result passing in teh model
      bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
      return Task.CompletedTask;

    }
  }
}
