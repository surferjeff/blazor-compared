gzipDotNet() {
    tar -czvf $1-src.tar.gz $(
        find $1 -name "*.cs" -o -name "*.cshtml" -o -name "*.razor" -o -name "*.fs" \
        | grep -vE "/(obj|bin)/"  | grep -v "Error")
}

gzipDotNet BlazorApp
gzipDotNet RazorApp
gzipDotNet GiraffeApp

tar -czvf GoApp-src.tar.gz GoApp/*.go GoApp/templates/*
tar -czvf GoTemplApp-src.tar.gz $(
    find GoTemplApp -name "*.go" -o -name "*.templ" | grep -v _templ.go)