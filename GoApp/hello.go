package main

import (
	"context"
	"io"
	"log"
	"time"

	"github.com/a-h/templ"
	"github.com/gofiber/fiber/v2"
	"github.com/gofiber/fiber/v2/middleware/logger"
)

type TemplViews struct {
}

func (v *TemplViews) Load() error {
	return nil
}

func (v *TemplViews) Render(w io.Writer, templateName string,
	data interface{}, args ...string) error {
	component := data.(templ.Component)
	return component.Render(context.Background(), w)
}

// Render Component.
func RenderC(c *fiber.Ctx, component templ.Component) error {
	return c.Render("", component)
}

// Wraps with Layout.
func RenderPage(c *fiber.Ctx, title string,
	component templ.Component) error {
	main := mainLayout(navMenu(c.Route().Path), component)
	headers := c.GetReqHeaders()
	layout := layout(title, main)
	if headers["Hx-Boosted"] == "true" {
		layout = boostedLayout(title, main)
	}
	return RenderC(c, layout)
}

func main() {
	templViews := new(TemplViews)
	app := fiber.New(fiber.Config{
		Views: templViews,
	})
	app.Use(logger.New())
	app.Static("/", "./wwwroot")

	app.Get("/", func(c *fiber.Ctx) error {
		c.Vary("HX-Boosted")
		return RenderPage(c, "Home", index())
	})
	app.Get("/about", func(c *fiber.Ctx) error {
		c.Vary("HX-Boosted")
		return RenderPage(c, "About", about())
	})
	app.Get("/counter", func(c *fiber.Ctx) error {
		c.Vary("HX-Boosted")
		return RenderPage(c, "Counter", counter(0))
	})
	app.Get("/increment", func(c *fiber.Ctx) error {
		count := c.QueryInt("count", 0)
		return RenderC(c, counter(count))
	})
	app.Get("/fetchdata", func(c *fiber.Ctx) error {
		c.Vary("HX-Boosted")
		return RenderPage(c, "Weather forecast", fetchData())
	})
	app.Post("/forecasts", func(c *fiber.Ctx) error {
		return RenderC(c, forecasts(getForecasts(time.Now())))
	})

	log.Fatal(app.Listen(":3000"))
}
