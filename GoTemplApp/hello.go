package main

import (
	"fmt"
	"log"
	"time"

	"github.com/a-h/templ"
	"github.com/gofiber/fiber/v2"
	"github.com/gofiber/fiber/v2/middleware/csrf"
	"github.com/gofiber/fiber/v2/middleware/logger"
	"github.com/gofiber/fiber/v2/middleware/session"
	"github.com/valyala/bytebufferpool"
)

// Render Component.
func RenderC(c *fiber.Ctx, component templ.Component) error {
	// Get new buffer from pool
	buf := bytebufferpool.Get()
	defer bytebufferpool.Put(buf)
	if err := component.Render(c.Context(), buf); err != nil {
		return fmt.Errorf("failed to render: %w", err)
	}

	c.Set("Content-Type", "text/html")
	c.Context().SetBody(buf.Bytes())

	return nil
}

// Wraps with Layout.
func RenderPage(c *fiber.Ctx, title string,
	component templ.Component) error {
	c.Vary("HX-Boosted")
	main := mainLayout(navMenu(c.Route().Path), component)
	headers := c.GetReqHeaders()
	var whichLayout templ.Component
	if headers["Hx-Boosted"] == "true" {
		whichLayout = boostedLayout(title, main)
	} else {
		whichLayout = layout(title, main)
	}
	return RenderC(c, whichLayout)
}

type IncrementForm struct {
	Count int
}

func main() {
	app := fiber.New(fiber.Config{})
	session := session.New()
	app.Use(logger.New())
	app.Use(csrf.New(csrf.Config{Session: session.Storage}))
	app.Static("/", "./wwwroot")

	app.Get("/", func(c *fiber.Ctx) error {
		return RenderPage(c, "Home", index())
	})
	app.Get("/about", func(c *fiber.Ctx) error {
		return RenderPage(c, "About", about())
	})
	app.Get("/counter", func(c *fiber.Ctx) error {
		return RenderPage(c, "Counter", counter(0))
	})
	app.Post("/increment", func(c *fiber.Ctx) error {
		form := IncrementForm{}
		c.BodyParser(&form)
		return RenderC(c, counter(form.Count))
	})
	app.Get("/fetchdata", func(c *fiber.Ctx) error {
		return RenderPage(c, "Weather forecast", fetchData())
	})
	app.Post("/forecasts", func(c *fiber.Ctx) error {
		return RenderC(c, forecasts(getForecasts(time.Now())))
	})

	log.Fatal(app.Listen(":3000"))
}
