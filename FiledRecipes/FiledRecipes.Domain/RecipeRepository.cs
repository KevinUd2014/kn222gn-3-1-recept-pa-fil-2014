﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FiledRecipes.Domain
{
    /// <summary>
    /// Holder for recipes.
    /// </summary>
    public class RecipeRepository : IRecipeRepository
    {
        /// <summary>
        /// Represents the recipe section.
        /// </summary>
        private const string SectionRecipe = "[Recept]";

        /// <summary>
        /// Represents the ingredients section.
        /// </summary>
        private const string SectionIngredients = "[Ingredienser]";

        /// <summary>
        /// Represents the instructions section.
        /// </summary>
        private const string SectionInstructions = "[Instruktioner]";

        /// <summary>
        /// Occurs after changes to the underlying collection of recipes.
        /// </summary>
        public event EventHandler RecipesChangedEvent;

        /// <summary>
        /// Specifies how the next line read from the file will be interpreted.
        /// </summary>
        private enum RecipeReadStatus { Indefinite, New, Ingredient, Instruction };

        /// <summary>
        /// Collection of recipes.
        /// </summary>
        private List<IRecipe> _recipes;

        /// <summary>
        /// The fully qualified path and name of the file with recipes.
        /// </summary>
        private string _path;

        /// <summary>
        /// Indicates whether the collection of recipes has been modified since it was last saved.
        /// </summary>
        public bool IsModified { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the RecipeRepository class.
        /// </summary>
        /// <param name="path">The path and name of the file with recipes.</param>
        public RecipeRepository(string path)
        {
            // Throws an exception if the path is invalid.
            _path = Path.GetFullPath(path);

            _recipes = new List<IRecipe>();
        }

        /// <summary>
        /// Returns a collection of recipes.
        /// </summary>
        /// <returns>A IEnumerable&lt;Recipe&gt; containing all the recipes.</returns>
        public virtual IEnumerable<IRecipe> GetAll()
        {
            // Deep copy the objects to avoid privacy leaks.
            return _recipes.Select(r => (IRecipe)r.Clone());
        }

        /// <summary>
        /// Returns a recipe.
        /// </summary>
        /// <param name="index">The zero-based index of the recipe to get.</param>
        /// <returns>The recipe at the specified index.</returns>
        public virtual IRecipe GetAt(int index)
        {
            // Deep copy the object to avoid privacy leak.
            return (IRecipe)_recipes[index].Clone();
        }

        /// <summary>
        /// Deletes a recipe.
        /// </summary>
        /// <param name="recipe">The recipe to delete. The value can be null.</param>
        public virtual void Delete(IRecipe recipe)
        {
            // If it's a copy of a recipe...
            if (!_recipes.Contains(recipe))
            {
                // ...try to find the original!
                recipe = _recipes.Find(r => r.Equals(recipe));
            }
            _recipes.Remove(recipe);
            IsModified = true;
            OnRecipesChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Deletes a recipe.
        /// </summary>
        /// <param name="index">The zero-based index of the recipe to delete.</param>
        public virtual void Delete(int index)
        {
            Delete(_recipes[index]);
        }

        /// <summary>
        /// Raises the RecipesChanged event.
        /// </summary>
        /// <param name="e">The EventArgs that contains the event data.</param>
        protected virtual void OnRecipesChanged(EventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            EventHandler handler = RecipesChangedEvent;

            // Event will be null if there are no subscribers. 
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

        public void Load()
           {
               List<IRecipe> List = new List<IRecipe>();//instansierar lista med Recept i metoden 1. Skapa lista som kan innehålla referenser till receptobjekt

                Recipe fullRecipe = null; // skapar en ny class variabel som alltid är null i detta stadie!
                RecipeReadStatus status = new RecipeReadStatus();//instansierar

                using (StreamReader Recipe = new StreamReader(@"Recipes.txt", System.Text.Encoding.UTF7))//öppnar receptet som jag vill inplementera i programmet! 2. Öppna textfilen för läsning.
                
                {// då denna metod ska visa alla recept så måste recipe ta både namn, ingredienser och instruktioner och det gör den från recipes!!

                string line; // en lokal variabel Line
                while((line = Recipe.ReadLine()) !=null)//medans Line är = Recipe gör detta! //för att man ska kunna läsa från receptet
                {

                    switch (line)// stringvariabel
                    {
                        case SectionRecipe:
                            status = RecipeReadStatus.New;//skapar ny, raden med new i dockumentet …sätt status till att nästa rad som läses in kommer att vara receptets namn.
                            continue;
                        case SectionIngredients://letar upp Ingrediet i text dokumentet! sätt status till att kommande rader som läses in kommer att vara receptets ingredienser.
                            status = RecipeReadStatus.Ingredient;
                            continue;
                        case SectionInstructions://letar upp Instructions i dokumentet! sätt status till att kommande rader som läses in kommer att vara receptets instruktioner.
                            status = RecipeReadStatus.Instruction;
                            continue;
                        default:

                            if (line != "")//om nu line är inget så kör den vidare
                            {


                                switch (status)
                                {
                                    case RecipeReadStatus.New:
                                        fullRecipe = new Recipe(line);//sparar Line som Fullrecipe och sparar detta! till listan!
                                        List.Add(fullRecipe); //lägger till i listan!
                                        break;
                                    case RecipeReadStatus.Ingredient:
                                        string[] splitedIngredients = line.Split(new string[] { ";" }, StringSplitOptions.None); //skapar en array som vi sparar den nya infon i! för att kunna redigera
                                        if (splitedIngredients.Length % 3 != 0)//om det finns mer än 3 st ; så kommer programmet kasta ett undantag!!
                                        {
                                            throw new FileFormatException();
                                        }
                                        Ingredient ingredient = new Ingredient();
                                        ingredient.Amount = splitedIngredients[0];// tar bort alla konstiga siffror och tecken med detta och fixar [0][1][2]
                                        ingredient.Measure = splitedIngredients[1];
                                        ingredient.Name = splitedIngredients[2];
                                        fullRecipe.Add(ingredient); //lägger till receptets lista i Ingredienser
                                        break;
                                    case RecipeReadStatus.Instruction:
                                        fullRecipe.Add(line);// lägger till raderna i listan med instruktioner
                                        break;
                                    case RecipeReadStatus.Indefinite:// är något fel med koden eller det som inplementeras så kastas här ett undantag!!
                                        throw new FileFormatException();
                                }
                            }
                            break;
                    }
                }
            }
                 //4. Sortera listan med recept med avseende på receptens namn.
                 //5. Tilldela avsett fält i klassen, _recipes, en referens till listan.
                _recipes = List.OrderBy(recipe => recipe.Name).ToList();//sorterar listan med recept beroende på namn!! tilldelar även ett fält i klassen (_recipes) till listan!
                IsModified = false;// 6. Tilldela avsedd egenskap i klassen, IsModified, ett värde som indikerar att listan med recept är oförändrad (false).
                OnRecipesChanged(EventArgs.Empty);//7. Utlös händelse om att recept har lästs in genom att anropa metoden OnRecipesChanged och skicka med parametern EventArgs.Empty
        }

        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(@"Recipes.txt", false, System.Text.Encoding.Default)) //ska skriva receptet och spara det!
            {
                foreach (IRecipe recipe in _recipes)
                {
                    writer.WriteLine(SectionRecipe); // skriver det valda receptet!
                    writer.WriteLine(recipe.Name);
                    writer.WriteLine(SectionIngredients);

                    foreach (IIngredient ingredient in recipe.Ingredients)
                    {
                        writer.WriteLine("{0};{1};{2}", ingredient.Amount, ingredient.Measure, ingredient.Name);
                    }

                    writer.WriteLine(SectionInstructions);

                    foreach (string instruction in recipe.Instructions)
                    {
                        writer.WriteLine(instruction);
                    }
                }
            }
            IsModified = false;
            OnRecipesChanged(EventArgs.Empty);
        }
    }
}