using System;
using System.Collections.Generic;

namespace LocalizationSystem
{
    [Serializable]
    public class LocalizationMetadata
    {
        public List<string> locales;
        public List<string> tables;
    }
}