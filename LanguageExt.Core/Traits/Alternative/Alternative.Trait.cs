namespace LanguageExt.Traits;

public interface Alternative<F> : Choice<F>, MonoidK<F>
    where F : Alternative<F>;
