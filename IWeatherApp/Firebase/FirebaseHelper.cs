﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Windows.UI.Popups;

namespace IWeatherApp
{
    class FirebaseHelper
    {
        public List<FirebaseObject<Favorite>> Cities { get; set; }
        private FirebaseClient _firebase;

        internal class Favorite
        {
            public string name;
            public int id;

            public Favorite(string cityName, int cityId)
            {
                this.name = cityName;
                this.id = cityId;
            }
        }

        public FirebaseHelper()
        {
            // create new firebase client based on auth token from currently logged user
            this._firebase = new FirebaseClient(
                Credentials.FirebaseURL,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => LoginAsync()
                });
        }

        private static async Task<string> LoginAsync()
        {
            // moze ten token trzeba odnawiac ???
            var token = UserService.Singleton.UserData.FirebaseToken;
            return token;
        }

        // wywolywane testowo w WeatherForecastViewModel.cs
        public async Task GetFavoritesCities()
        {
            string userId = UserService.Singleton.UserData.User.LocalId;

            // cities - list of the favorites cities for the current user
            Cities = (List<FirebaseObject<Favorite>>)await _firebase
                .Child(userId).Child("favourites").OrderByKey().OnceAsync<Favorite>();
            string tmp = "";
            foreach (var city in Cities)
            {
                tmp += $"{city.Object.id} is {city.Object.name} || ";
            }

            await new MessageDialog(tmp).ShowAsync();
        }

        public async Task PutFavoriteCity(string cityName, int cityId)
        {
            string userId = UserService.Singleton.UserData.User.LocalId;

            // get current list of favorites cities
            await GetFavoritesCities();

            if (Cities.Count < Constants.MaxNumberOfFavoritesCities)
            {
                // create new city to add to favorites list
                Favorite newFavoriteCity = new Favorite(cityName, cityId);

                // add city to favorites
                await _firebase.Child(userId).Child("favourites").Child(cityId.ToString()).PutAsync(newFavoriteCity);
            }
            else
            {
                await new MessageDialog(Constants.MaxNumberOfFavoritesCitiesExceededErrorMessage).ShowAsync();
            }
        }

        public async Task DeleteCityFromFavorites(int cityId)
        {
            // get id of the current user
            string userId = UserService.Singleton.UserData.User.LocalId;

            // get current list of favorites cities
            await GetFavoritesCities();

            // check if given city exists in favorites list
            foreach (var city in Cities)
            {
                if (city.Object.id == cityId)
                {
                    // delete the city
                    await _firebase.Child(userId).Child("favourites").Child(cityId.ToString()).DeleteAsync();

                    return;  // end task after finding the city
                }
            }

            // if given city doesn't exist in favorites lists, raise an error
            await new MessageDialog(Constants.CityDoesntExistsInFavoritesErrorMessage).ShowAsync();
        }
    }

    
}
