namespace OfficeIMO.Markdown;

/// <summary>
/// Context available while rendering a markdown document body to HTML.
/// </summary>
public sealed class MarkdownBodyRenderContext {
    internal MarkdownBodyRenderContext(
        IReadOnlyList<IMarkdownBlock> blocks,
        HtmlOptions options,
        MarkdownHeadingCatalog headingCatalog) {
        Blocks = blocks;
        Options = options;
        HeadingCatalog = headingCatalog;
    }

    /// <summary>
    /// Top-level blocks being rendered for the current body.
    /// </summary>
    public IReadOnlyList<IMarkdownBlock> Blocks { get; }

    /// <summary>
    /// Active HTML rendering options.
    /// </summary>
    public HtmlOptions Options { get; }

    internal MarkdownHeadingCatalog HeadingCatalog { get; }

    /// <summary>
    /// Returns the zero-based index of a top-level block in <see cref="Blocks"/>, or <c>-1</c> when the block is not present.
    /// </summary>
    public int GetBlockIndex(IMarkdownBlock block) {
        if (block == null) {
            return -1;
        }

        for (int i = 0; i < Blocks.Count; i++) {
            if (ReferenceEquals(Blocks[i], block)) {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns the resolved anchor id for a heading block within the current rendered body.
    /// </summary>
    public string GetHeadingAnchor(IMarkdownBlock heading) =>
        heading is IHeadingMarkdownBlock headingBlock
            ? HeadingCatalog.GetHeadingAnchor(headingBlock)
            : string.Empty;

    /// <summary>
    /// Returns the anchor id of the nearest preceding heading according to the supplied TOC options,
    /// or <c>null</c> when no heading title should be associated with the specified block index.
    /// </summary>
    public string? GetPrecedingHeadingAnchor(int blockIndex, TocOptions options) =>
        HeadingCatalog.GetPrecedingHeadingAnchor(Blocks, blockIndex, options ?? new TocOptions());

    /// <summary>
    /// Builds TOC-style heading entries relative to a specific top-level block index using the supplied TOC options.
    /// </summary>
    public IReadOnlyList<TocBlock.Entry> BuildTocEntries(int blockIndex, TocOptions options, string? titleAnchor = null) =>
        HeadingCatalog.BuildTocEntries(Blocks, blockIndex, options ?? new TocOptions(), titleAnchor);
}
