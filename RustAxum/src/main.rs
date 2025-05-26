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

#[derive(Template)]
#[template(path = "goodday.html")]
struct GoodDayTemplate { }

#[derive(Template)]
#[template(path = "layout.html")]
struct LayoutTemplate<'a, T: Template> {
    title: &'a str,
    main_html: &'a T
}

trait IntoHtml {
    fn into_html(self) -> impl IntoResponse;
}

impl IntoHtml for askama::Result<String> {
    fn into_html(self) -> impl IntoResponse {
        match self {
            Ok(html) => Html(html).into_response(),
            Err(err) => (
                StatusCode::INTERNAL_SERVER_ERROR,
                format!("Failed to render template. Error: {err}"),
            ).into_response(),
        }        
    }
}

async fn hello(Path(name): Path<String>) -> impl IntoResponse {
    let template = HelloTemplate { name: name.as_str() };
    template.render().into_html()
}

async fn goodday() -> impl IntoResponse {
    let goodday = GoodDayTemplate {};
    let template = LayoutTemplate {
        title: "Good Day",
        main_html: &goodday
    };
    template.render().into_html()
}

#[tokio::main]
async fn main() {
    let app = Router::new()
        .route("/hello/{name}", get(hello))
        .route("/goodday", get(goodday))
        .fallback_service(ServeDir::new("static/"));

    let listener = tokio::net::TcpListener::bind("0.0.0.0:3456").await.unwrap();
    axum::serve(listener, app).await.unwrap();
}