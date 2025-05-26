use axum::extract::Path;
use axum::http::StatusCode;
use axum::response::{Html, IntoResponse};
use axum::routing::get;
use axum::Router;
use askama::Template;
use tower_http::services::ServeDir;

#[derive(Template)]
#[template(path = "index.html")]
struct HelloTemplate<'a> {
    name: &'a str,
}

// #[derive(Template)]
// #[template(path = "layout.html")]
// struct LayoutTemplate<'a, T: Template> {
//     title: &'a str,
//     main_html: &'a T
// }



async fn hello(Path(name): Path<String>) -> impl IntoResponse {
    let template = HelloTemplate { name: name.as_str() };
    match template.render() {
        Ok(html) => Html(html).into_response(),
        Err(err) => (
            StatusCode::INTERNAL_SERVER_ERROR,
            format!("Failed to render template. Error: {err}"),
        ).into_response(),
    }
}

#[tokio::main]
async fn main() {
    let app = Router::new()
        .route("/hello/{name}", get(hello))
        .fallback_service(ServeDir::new("static/"));

    let listener = tokio::net::TcpListener::bind("0.0.0.0:3456").await.unwrap();
    axum::serve(listener, app).await.unwrap();
}