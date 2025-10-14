using System.Text;
using System.Text.Json;

namespace InvenireServer.Presentation.Policies;

public class JsonSnakeCasePolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var builder = new StringBuilder(name.Length + 10);
        for (int i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (char.IsUpper(c))
            {
                if (i > 0) builder.Append('_');
                builder.Append(char.ToLowerInvariant(c));
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }
}
