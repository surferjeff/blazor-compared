use axum::extract::OriginalUri;
use axum::http::{HeaderMap, StatusCode};
use axum::response::{Html, IntoResponse};
use axum::routing::{get, post};
use axum::{Form, Router};
use askama::Template;
use chrono::{Duration, Local};
use rand::Rng;
use serde::Deserialize;
use tower_http::services::ServeDir;

#[derive(Template)]
#[template(path = "layout.html")]
struct LayoutTemplate<'a, T: Template> {
    title: &'a str,
    main_html: &'a T
}

#[derive(Template)]
#[template(path = "main_layout.html")]
struct MainLayoutTemplate<'a, NM: Template, MA:Template> {
    nav_menu_html: &'a NM,
    main_article_html: &'a MA
}

#[derive(Template)]
#[template(path = "boosted_layout.html")]
struct BoostedLayoutTemplate<'a, MA:Template> {
    title: &'a str,
    main_article_html: &'a MA
}

#[derive(Template)]
#[template(path = "nav_menu.html")]
struct NavMenuTemplate<'a> {
    path: &'a str,
}

#[derive(Template)]
#[template(path = "about.html")]
struct AboutTemplate { }

#[derive(Template)]
#[template(path = "home.html")]
struct HomeTemplate { }

#[derive(Template)]
#[template(path = "fetchdata.html")]
struct FetchDataTemplate { }

#[derive(Template)]
#[template(path = "survey.html")]
struct SurveyTemplate<'a> { 
    title: &'a str
}

#[derive(Template, Deserialize)]
#[template(path = "counter.html")]
struct CounterTemplate { 
    count: u32
}

#[derive(Template)]
#[template(path = "forecasts.html")]
struct ForecastsTemplate { 
    forecasts: [Forecast; 5]
}


impl HomeTemplate {
    pub fn survey_html<'a> (&'a self, title: &'a str) -> SurveyTemplate<'a> {
        SurveyTemplate { title }
    }
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
        ; todo!("Set the Vary: HxBoosted header.")
    }
}

async fn index(uri: OriginalUri, headers: HeaderMap) -> impl IntoResponse {
    render_main_layout_page("Home", uri.0.path(), headers, &HomeTemplate {})
}

async fn about(uri: OriginalUri, headers: HeaderMap) -> impl IntoResponse {
    render_main_layout_page("About", uri.0.path(), headers, &AboutTemplate {})
}

async fn counter(uri: OriginalUri, headers: HeaderMap) -> impl IntoResponse {
    render_main_layout_page("Counter", uri.0.path(), headers, &CounterTemplate { count: 0 })
}

async fn fetchdata(uri: OriginalUri, headers: HeaderMap) -> impl IntoResponse {
    render_main_layout_page("Fetch Data", uri.0.path(), headers, &FetchDataTemplate {})
}

async fn increment(Form(form): Form<CounterTemplate>) -> impl IntoResponse {
    form.render().into_html()
}

struct Forecast {
    date: String,
    summary: &'static str,
    temperature_c: i32,
    temperature_f: i32,
}

fn make_forecasts() -> [Forecast; 5] {
    let today = Local::now().date_naive();
    let summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm",
        "Balmy", "Hot", "Sweltering", "Scorching"];

    let mut rng = rand::rng();
    let mut make_forecast = |day| {
        let c: i32 = rng.random_range(40..=90);
        Forecast {
            date: (today + Duration::days(day)).to_string(),
            summary: summaries[rng.random_range(0..summaries.len())],
            temperature_c: c,
            temperature_f: (32.0 + (c as f32)/0.5556) as i32,
        }
    };
    [
        make_forecast(1),
        make_forecast(2),
        make_forecast(3),
        make_forecast(4),
        make_forecast(5),
    ]
}

async fn forecasts() -> impl IntoResponse {
    ForecastsTemplate { forecasts: make_forecasts() }.render().into_html()
}

fn render_main_layout_page<'a, MA>(
    title: &'a str,
    path: &'a str,
    headers: HeaderMap,
    main_article_html: &'a MA,
) -> impl IntoResponse
where
    MA: Template,
{
    if let Some(Ok("true")) = headers.get("HX-Boosted").map(|v| v.to_str()) {
        BoostedLayoutTemplate {
            title,
            main_article_html: &MainLayoutTemplate {
                nav_menu_html: &NavMenuTemplate { path },
                main_article_html,
            }
        }.render().into_html()
    } else {
        let main_layout = MainLayoutTemplate {
            nav_menu_html: &NavMenuTemplate { path },
            main_article_html,
        };
        LayoutTemplate {
            title,
            main_html: &main_layout,
        }
        .render()
        .into_html()
    }
}



#[tokio::main]
async fn main() {
    let app = Router::new()
        .route("/",  get(index))
        .route("/about", get(about))
        .route("/counter", get(counter))
        .route("/increment", post(increment))
        .route("/fetchdata", get(fetchdata))
        .route("/forecasts", post(forecasts))
        .fallback_service(ServeDir::new("static/"));

    let listener = tokio::net::TcpListener::bind("0.0.0.0:3456").await.unwrap();
    axum::serve(listener, app).await.unwrap();
}