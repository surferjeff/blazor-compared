use axum::{routing::get, Router};

async fn hello() -> &'static str {
    "Hello, world!"
}

#[tokio::main]
async fn main() {
    let app = Router::new()
        .route("/hello", get(hello));

    let listener = tokio::net::TcpListener::bind("0.0.0.0:3456").await.unwrap();
    axum::serve(listener, app).await.unwrap();
}