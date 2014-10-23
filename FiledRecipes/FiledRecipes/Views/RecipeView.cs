using FiledRecipes.Domain;
using FiledRecipes.App.Mvp;
using FiledRecipes.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiledRecipes.Views
{
    /// <summary>
    /// 
    /// </summary>
    public class RecipeView : ViewBase, IRecipeView
    {
        public void Show(IRecipe recipe)
        {
            Header = recipe.Name; // anropar metoden Header och gör den = recept namnet!
            ShowHeaderPanel();//skriver ut text-headern!

            foreach (Ingredient ingredientz in recipe.Ingredients) // tar från klassen ingredient och skapar en ny variabel, "infon" kommer från Recipe.ingredients
            {
                Console.WriteLine(ingredientz);// skriver ut den informationen om receptet från ingredienser!
            }

            foreach (string instructionz in recipe.Instructions)//hämtar en sträng från recipe.instructions och skapar en ny variabel
            {
                Console.WriteLine(instructionz); //skriver ut!
            }
            
            }
        public void Show(IEnumerable<IRecipe> recipes)
        {
            foreach (string Name in )
            {
                Console.WriteLine();
            }
            ContinueOnKeyPressed();
        }
    }
}
