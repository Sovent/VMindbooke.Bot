using System;
using Bogus;
using Bogus.DataSets;

namespace Usage
{
    class WordsGenerator : IWordsGenerator
    {
        private readonly Lorem _generator;

        public static WordsGenerator Create(string local)
        {
            Randomizer.Seed = CreateSeed();
            var generator = new Lorem(local);
            return new WordsGenerator(generator);
        }

        public string GetTitle()
        {
            var title = _generator.Sentence(new Random().Next() % 2 + 10);
            return title;
        }

        public string GetContent()
        {
            var title = _generator.Sentence(new Random().Next() % 5 + 30);
            return title;
        }

        private WordsGenerator(Lorem generator)
        {
            _generator = generator;
        }

        private static Random CreateSeed()
        {
            return new Random(Guid.NewGuid().GetHashCode());
        }
    }
}