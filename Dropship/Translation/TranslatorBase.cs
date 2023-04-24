using System.Collections.Generic;

namespace Dropship.Translation;

public abstract class TranslatorBase
{
    public virtual int Priority => 0;

    public virtual SupportedLangs DefaultLang => SupportedLangs.English;

    public abstract bool IsSupport(SupportedLangs languageId);

    public abstract Dictionary<string, string> GetTranslation(SupportedLangs languageId);
}
