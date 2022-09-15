using System;
namespace JokesWebApp.Models
{
    public class Joke{
        public int Id { get; set; } = default!;
        public string JokeQuestion { get; set; } = default!;
        public string JokeAnswer { get; set; } = default!;

        public Joke()
        {

        }

    }
}

