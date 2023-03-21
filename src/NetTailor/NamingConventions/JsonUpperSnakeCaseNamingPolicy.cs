using System.Text.Json;

namespace NetTailor.NamingConventions;

internal class JsonUpperSnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        int nameLength = name.Length;
        int estimatedLength = nameLength;

        for (int i = 1; i < nameLength; i++)
        {
            if (char.IsUpper(name[i]) &&
                (char.IsLower(name[i - 1]) || (i < nameLength - 1 && char.IsLower(name[i + 1]))) && name[i - 1] != '_')
            {
                estimatedLength++;
            }
        }

#if NETCOREAPP
        return string.Create(estimatedLength, name, (chars, srcName) =>
        {
            int targetIndex = 0;
            for (int srcIndex = 0; srcIndex < srcName.Length; srcIndex++)
            {
                char currentChar = srcName[srcIndex];

                if (srcIndex > 0 && char.IsUpper(currentChar) &&
                    (char.IsLower(srcName[srcIndex - 1]) || (srcIndex < srcName.Length - 1 && char.IsLower(srcName[srcIndex + 1]))) && srcName[srcIndex - 1] != '_')
                {
                    chars[targetIndex++] = '_';
                }

                chars[targetIndex++] = char.ToUpperInvariant(currentChar);
            }
        });
#else
        char[] buffer = new char[estimatedLength];
        int bufferIndex = 0;

        for (int i = 0; i < nameLength; i++)
        {
            char currentChar = name[i];

            if (i > 0 && char.IsUpper(currentChar) &&
                (char.IsLower(name[i - 1]) || (i < nameLength - 1 && char.IsLower(name[i + 1]))) && name[i - 1] != '_')
            {
                buffer[bufferIndex++] = '_';
            }

            buffer[bufferIndex++] = char.ToUpperInvariant(currentChar);
        }

        return new string(buffer, 0, bufferIndex);
#endif
    }
}

