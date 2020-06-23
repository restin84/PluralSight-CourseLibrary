using CourseLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.ValidationAttributes
{
  public class CourseTitleMustBeDifferentFromTheDescriptionAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
      //in this case we are validating at the class level so object and validationContext refer to the same thing
      //if we were instead validating a property then value would refer to the object that the property we 
      //are validating belongs to and validationContext will refer to the property that we are validating
      var course = (CourseForCreationDto)validationContext.ObjectInstance;

      if (course.Title == course.Description) {
        return new ValidationResult(
            "The provided desciption should be different from the title.",
            new [] {nameof(CourseForCreationDto)}
          );
      }

      return ValidationResult.Success;
    }
  }
}
