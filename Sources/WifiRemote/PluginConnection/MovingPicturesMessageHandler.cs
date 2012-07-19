using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.PluginConnection
{
    /// <summary>
    /// Handler for MovingPictures messages
    /// </summary>
    internal class MovingPicturesMessageHandler
    {
        /// <summary>
        /// Handle an MovingPictures message received from a client
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="socketServer">Socket server</param>
        /// <param name="sender">Sender</param>
        internal static void HandleMovingPicturesMessage(Newtonsoft.Json.Linq.JObject message, SocketServer socketServer, Deusty.Net.AsyncSocket sender)
        {
            string action = (string)message["Action"];

            if (!string.IsNullOrEmpty(action))
            {
                // Show movie details for this movie
                if (action == "moviedetails")
                {
                    string movieName = (string)message["MovieName"];
                    if (!string.IsNullOrEmpty(movieName))
                    {
                        int movieId = MovingPicturesHelper.GetMovieIdByName(movieName);
                        MovingPicturesHelper.ShowMovieDetails(movieId);
                    }
                }
                // Play a movie with MovingPictures
                else if (action == "playmovie")
                {
                    int movieId = (message["MovieId"] != null) ? (int)message["MovieId"] : -1;
                    string movieName = (string)message["MovieName"];
                    bool resume = (message["AskToResume"] != null) ? (bool)message["AskToResume"] : true;
                    int startPos = (message["StartPosition"] != null) ? (int)message["StartPosition"] : 0;

                    // Play by movie id
                    if (movieId != -1)
                    {
                        MovingPicturesHelper.PlayMovie(movieId, resume, startPos);
                    }
                    else if (!string.IsNullOrEmpty(movieName))
                    {
                        // Play by name
                        MovingPicturesHelper.PlayMovie(movieName, resume, startPos);
                    }
                }
            }
        }
    }
}
