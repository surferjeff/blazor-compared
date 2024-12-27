module Css

open System
open System.Collections.Generic
open System.IO.Hashing
open System.Text

[<Struct>]
type Property = {
    key: string
    value: string
}

let property (key: string) (value: string) = 
    { key = key; value = value }

[<Struct>]
type Block = {
    MediaQuery: string
    PropText: string
    Hash: uint64
}

let blockFrom (mediaQuery: string) (propText: string) =
    let hasher = XxHash64()
    hasher.Append(Encoding.UTF8.GetBytes(mediaQuery))
    hasher.Append(Encoding.UTF8.GetBytes(propText))
    { 
        MediaQuery = mediaQuery
        PropText = propText
        Hash = hasher.GetCurrentHashAsUInt64()
    }

[<Struct>]
type ClassDef = {
    Blocks: Block list
    Name: string
    Hash: uint64
}

let private defaultClassDef = {
    Blocks = []
    Name = ""
    Hash = 0UL
}

let private propTextFrom (props: Property list) =
    props
    |> Seq.fold (fun lines prop -> $"\t{prop.key}: {prop.value};" :: lines) []
    |> String.concat "\n"

let scopedClass (namePrefix: string) =
    { defaultClassDef with Name = namePrefix }

let media (media: string) (props: Property list) (classDef: ClassDef) =
    let block = blockFrom media (propTextFrom props)
    { classDef with
        Blocks = block :: classDef.Blocks
        Hash = classDef.Hash ^^^ block.Hash
    }

let mediaAll (props: Property list) (classDef: ClassDef) =
    media "" props classDef

let private stringFromHash (hash: uint64) =
    let chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-"
    let result, _ = 
        {1..6}
        |> Seq.mapFold (fun hash _ ->
            chars.[int (hash % uint64 chars.Length)], hash / uint64 chars.Length) hash
    result |> String.Concat

[<Struct>]
type private ClassDecl = {
    Media: string
    ClassName: string
    PropText: string
}

type Head() =
    let classNames = SortedDictionary<string, string>()
    let classProps = SortedSet<ClassDecl>()

    member this.Add (classDef: ClassDef) =
        let className =
            match classNames.TryGetValue classDef.Name with
            | true, name -> name
            | _ -> 
                let name = $"{classDef.Name}-{stringFromHash classDef.Hash}"
                classNames.Add(classDef.Name, name)
                name

        for query in classDef.Blocks do
            let headKey = {
                Media = query.MediaQuery
                ClassName = className
                PropText = query.PropText
            }
            classProps.Add headKey |> ignore
            
        className

    member this.toStyleText() =
        let mutable lines = []
        let mutable prevQuery = None
        for classDecl in classProps do
            match prevQuery, classDecl.Media with
            | None, "" -> () // The first group has no enclosing media query.
            | None, query ->
                lines <- $"@media {query} {{" :: lines
                prevQuery <- Some query
            | Some prev, query when prev = query -> () // Continue in same media.
            | Some _, query ->
                lines <- $"@media {query} {{" :: "}" :: lines
                prevQuery <- Some query
            lines <- "}" :: classDecl.PropText :: $".{classDecl.ClassName} {{" :: lines
        if prevQuery |> Option.isSome then
            lines <- "}" :: lines
        lines |> List.rev |> String.concat "\n"


let alignContent = property "align-content"
let alignItems = property "align-items"
let alignSelf = property "align-self"
let all = property "all"
let animation = property "animation"
let animationDelay = property "animation-delay"
let animationDirection = property "animation-direction"
let animationDuration = property "animation-duration"
let animationFillMode = property "animation-fill-mode"
let animationIterationCount = property "animation-iteration-count"
let animationName = property "animation-name"
let animationPlayState = property "animation-play-state"
let animationTimingFunction = property "animation-timing-function"
let azimuth = property "azimuth"
let background = property "background"
let backgroundAttachment = property "background-attachment"
let backgroundBlendMode = property "background-blend-mode"
let backgroundClip = property "background-clip"
let backgroundColor = property "background-color"
let backgroundImage = property "background-image"
let backgroundOrigin = property "background-origin"
let backgroundPosition = property "background-position"
let backgroundRepeat = property "background-repeat"
let backgroundSize = property "background-size"
let border = property "border"
let borderBottom = property "border-bottom"
let borderBottomColor = property "border-bottom-color"
let borderBottomLeftRadius = property "border-bottom-left-radius"
let borderBottomRightRadius = property "border-bottom-right-radius"
let borderBottomStyle = property "border-bottom-style"
let borderBottomWidth = property "border-bottom-width"
let borderCollapse = property "border-collapse"
let borderColor = property "border-color"
let borderImage = property "border-image"
let borderImageOutset = property "border-image-outset"
let borderImageRepeat = property "border-image-repeat"
let borderImageSlice = property "border-image-slice"
let borderImageSource = property "border-image-source"
let borderImageWidth = property "border-image-width"
let borderLeft = property "border-left"
let borderLeftColor = property "border-left-color"
let borderLeftStyle = property "border-left-style"
let borderLeftWidth = property "border-left-width"
let borderRadius = property "border-radius"
let borderRight = property "border-right"
let borderRightColor = property "border-right-color"
let borderRightStyle = property "border-right-style"
let borderRightWidth = property "border-right-width"
let borderSpacing = property "border-spacing"
let borderStyle = property "border-style"
let borderTop = property "border-top"
let borderTopColor = property "border-top-color"
let borderTopLeftRadius = property "border-top-left-radius"
let borderTopRightRadius = property "border-top-right-radius"
let borderTopStyle = property "border-top-style"
let borderTopWidth = property "border-top-width"
let borderWidth = property "border-width"
let bottom = property "bottom"
let boxDecorationBreak = property "box-decoration-break"
let boxShadow = property "box-shadow"
let boxSizing = property "box-sizing"
let breakAfter = property "break-after"
let breakBefore = property "break-before"
let breakInside = property "break-inside"
let captionSide = property "caption-side"
let caretColor = property "caret-color"
let clear = property "clear"
let clip = property "clip"
let clipPath = property "clip-path"
let clipRule = property "clip-rule"
let color = property "color"
let colorInterpolationFilters = property "color-interpolation-filters"
let columnCount = property "column-count"
let columnFill = property "column-fill"
let columnGap = property "column-gap"
let columnRule = property "column-rule"
let columnRuleColor = property "column-rule-color"
let columnRuleStyle = property "column-rule-style"
let columnRuleWidth = property "column-rule-width"
let columns = property "columns"
let columnSpan = property "column-span"
let columnWidth = property "column-width"
let contain = property "contain"
let content = property "content"
let counterIncrement = property "counter-increment"
let counterReset = property "counter-reset"
let cue = property "cue"
let cueAfter = property "cue-after"
let cueBefore = property "cue-before"
let cursor = property "cursor"
let direction = property "direction"
let display = property "display"
let elevation = property "elevation"
let emptyCells = property "empty-cells"
let filter = property "filter"
let flex = property "flex"
let flexBasis = property "flex-basis"
let flexDirection = property "flex-direction"
let flexFlow = property "flex-flow"
let flexGrow = property "flex-grow"
let flexShrink = property "flex-shrink"
let flexWrap = property "flex-wrap"
let float = property "float"
let floodColor = property "flood-color"
let floodOpacity = property "flood-opacity"
let font = property "font"
let fontFamily = property "font-family"
let fontFeatureSettings = property "font-feature-settings"
let fontKerning = property "font-kerning"
let fontLanguageOverride = property "font-language-override"
let fontOpticalSizing = property "font-optical-sizing"
let fontPalette = property "font-palette"
let fontSize = property "font-size"
let fontSizeAdjust = property "font-size-adjust"
let fontStretch = property "font-stretch"
let fontStyle = property "font-style"
let fontSynthesis = property "font-synthesis"
let fontSynthesisPosition = property "font-synthesis-position"
let fontSynthesisSmallCaps = property "font-synthesis-small-caps"
let fontSynthesisStyle = property "font-synthesis-style"
let fontSynthesisWeight = property "font-synthesis-weight"
let fontVariant = property "font-variant"
let fontVariantAlternates = property "font-variant-alternates"
let fontVariantCaps = property "font-variant-caps"
let fontVariantEastAsian = property "font-variant-east-asian"
let fontVariantEmoji = property "font-variant-emoji"
let fontVariantLigatures = property "font-variant-ligatures"
let fontVariantNumeric = property "font-variant-numeric"
let fontVariantPosition = property "font-variant-position"
let fontVariationSettings = property "font-variation-settings"
let fontWeight = property "font-weight"
let gap = property "gap"
let glyphOrientationVertical = property "glyph-orientation-vertical"
let grid = property "grid"
let gridArea = property "grid-area"
let gridAutoColumns = property "grid-auto-columns"
let gridAutoFlow = property "grid-auto-flow"
let gridAutoRows = property "grid-auto-rows"
let gridColumn = property "grid-column"
let gridColumnEnd = property "grid-column-end"
let gridColumnGap = property "grid-column-gap"
let gridColumnStart = property "grid-column-start"
let gridGap = property "grid-gap"
let gridRow = property "grid-row"
let gridRowEnd = property "grid-row-end"
let gridRowGap = property "grid-row-gap"
let gridRowStart = property "grid-row-start"
let gridTemplate = property "grid-template"
let gridTemplateAreas = property "grid-template-areas"
let gridTemplateColumns = property "grid-template-columns"
let gridTemplateRows = property "grid-template-rows"
let hangingPunctuation = property "hanging-punctuation"
let height = property "height"
let hyphens = property "hyphens"
let imageOrientation = property "image-orientation"
let imageRendering = property "image-rendering"
let isolation = property "isolation"
let justifyContent = property "justify-content"
let justifyItems = property "justify-items"
let justifySelf = property "justify-self"
let left = property "left"
let letterSpacing = property "letter-spacing"
let lightingColor = property "lighting-color"
let lineBreak = property "line-break"
let lineHeight = property "line-height"
let listStyle = property "list-style"
let listStyleImage = property "list-style-image"
let listStylePosition = property "list-style-position"
let listStyleType = property "list-style-type"
let margin = property "margin"
let marginBottom = property "margin-bottom"
let marginLeft = property "margin-left"
let marginRight = property "margin-right"
let marginTop = property "margin-top"
let mask = property "mask"
let maskBorder = property "mask-border"
let maskBorderMode = property "mask-border-mode"
let maskBorderOutset = property "mask-border-outset"
let maskBorderRepeat = property "mask-border-repeat"
let maskBorderSlice = property "mask-border-slice"
let maskBorderSource = property "mask-border-source"
let maskBorderWidth = property "mask-border-width"
let maskClip = property "mask-clip"
let maskComposite = property "mask-composite"
let maskImage = property "mask-image"
let maskMode = property "mask-mode"
let maskOrigin = property "mask-origin"
let maskPosition = property "mask-position"
let maskRepeat = property "mask-repeat"
let maskSize = property "mask-size"
let maskType = property "mask-type"
let maxHeight = property "max-height"
let maxWidth = property "max-width"
let minHeight = property "min-height"
let minWidth = property "min-width"
let mixBlendMode = property "mix-blend-mode"
let objectFit = property "object-fit"
let objectPosition = property "object-position"
let order = property "order"
let orphans = property "orphans"
let outline = property "outline"
let outlineColor = property "outline-color"
let outlineOffset = property "outline-offset"
let outlineStyle = property "outline-style"
let outlineWidth = property "outline-width"
let overflow = property "overflow"
let overflowWrap = property "overflow-wrap"
let padding = property "padding"
let paddingBottom = property "padding-bottom"
let paddingLeft = property "padding-left"
let paddingRight = property "padding-right"
let paddingTop = property "padding-top"
let pageBreakAfter = property "page-break-after"
let pageBreakBefore = property "page-break-before"
let pageBreakInside = property "page-break-inside"
let pause = property "pause"
let pauseAfter = property "pause-after"
let pauseBefore = property "pause-before"
let pitch = property "pitch"
let pitchRange = property "pitch-range"
let placeContent = property "place-content"
let placeItems = property "place-items"
let placeSelf = property "place-self"
let playDuring = property "play-during"
let position = property "position"
let propertyName = property "property-name"
let quotes = property "quotes"
let resize = property "resize"
let rest = property "rest"
let restAfter = property "rest-after"
let restBefore = property "rest-before"
let richness = property "richness"
let right = property "right"
let rowGap = property "row-gap"
let scrollMargin = property "scroll-margin"
let scrollMarginBlock = property "scroll-margin-block"
let scrollMarginBlockEnd = property "scroll-margin-block-end"
let scrollMarginBlockStart = property "scroll-margin-block-start"
let scrollMarginBottom = property "scroll-margin-bottom"
let scrollMarginInline = property "scroll-margin-inline"
let scrollMarginInlineEnd = property "scroll-margin-inline-end"
let scrollMarginInlineStart = property "scroll-margin-inline-start"
let scrollMarginLeft = property "scroll-margin-left"
let scrollMarginRight = property "scroll-margin-right"
let scrollMarginTop = property "scroll-margin-top"
let scrollPadding = property "scroll-padding"
let scrollPaddingBlock = property "scroll-padding-block"
let scrollPaddingBlockEnd = property "scroll-padding-block-end"
let scrollPaddingBlockStart = property "scroll-padding-block-start"
let scrollPaddingBottom = property "scroll-padding-bottom"
let scrollPaddingInline = property "scroll-padding-inline"
let scrollPaddingInlineEnd = property "scroll-padding-inline-end"
let scrollPaddingInlineStart = property "scroll-padding-inline-start"
let scrollPaddingLeft = property "scroll-padding-left"
let scrollPaddingRight = property "scroll-padding-right"
let scrollPaddingTop = property "scroll-padding-top"
let scrollSnapAlign = property "scroll-snap-align"
let scrollSnapStop = property "scroll-snap-stop"
let scrollSnapType = property "scroll-snap-type"
let shapeImageThreshold = property "shape-image-threshold"
let shapeMargin = property "shape-margin"
let shapeOutside = property "shape-outside"
let speak = property "speak"
let speakAs = property "speak-as"
let speakHeader = property "speak-header"
let speakNumeral = property "speak-numeral"
let speakPunctuation = property "speak-punctuation"
let speechRate = property "speech-rate"
let stress = property "stress"
let tableLayout = property "table-layout"
let tabSize = property "tab-size"
let textAlign = property "text-align"
let textAlignAll = property "text-align-all"
let textAlignLast = property "text-align-last"
let textCombineUpright = property "text-combine-upright"
let textDecoration = property "text-decoration"
let textDecorationColor = property "text-decoration-color"
let textDecorationLine = property "text-decoration-line"
let textDecorationStyle = property "text-decoration-style"
let textEmphasis = property "text-emphasis"
let textEmphasisColor = property "text-emphasis-color"
let textEmphasisPosition = property "text-emphasis-position"
let textEmphasisStyle = property "text-emphasis-style"
let textIndent = property "text-indent"
let textJustify = property "text-justify"
let textOrientation = property "text-orientation"
let textOverflow = property "text-overflow"
let textShadow = property "text-shadow"
let textTransform = property "text-transform"
let textUnderlinePosition = property "text-underline-position"
let top = property "top"
let transform = property "transform"
let transformBox = property "transform-box"
let transformOrigin = property "transform-origin"
let transition = property "transition"
let transitionDelay = property "transition-delay"
let transitionDuration = property "transition-duration"
let transitionProperty = property "transition-property"
let transitionTimingFunction = property "transition-timing-function"
let unicodeBidi = property "unicode-bidi"
let verticalAlign = property "vertical-align"
let visibility = property "visibility"
let voiceBalance = property "voice-balance"
let voiceDuration = property "voice-duration"
let voiceFamily = property "voice-family"
let voicePitch = property "voice-pitch"
let voiceRange = property "voice-range"
let voiceRate = property "voice-rate"
let voiceStress = property "voice-stress"
let voiceVolume = property "voice-volume"
let volume = property "volume"
let whiteSpace = property "white-space"
let widows = property "widows"
let width = property "width"
let willChange = property "will-change"
let wordBreak = property "word-break"
let wordSpacing = property "word-spacing"
let wordWrap = property "word-wrap"
let writingMode = property "writing-mode"
let zIndex  = property "z-index "
