describe("select-laboratory-page", function () {
    beforeEach(function () {
        localStorage.setItem("refreshToken", "refreshToken123");
        cy.visit("http://localhost:8080/reports");
    });
    it("check visit by admin", function () {
        cy.intercept("POST", "".concat(Cypress.env("VITE_API_URL"), "/api/auth/refresh-token"), { fixture: "auth-admin-response.json" });
        cy.intercept("GET", "".concat(Cypress.env("VITE_API_URL"), "/api/laboratories/simplified"), {
            fixture: "laboratories-response.json",
        });
        cy.get("ul[id='laboratories-list']").children().should("have.length", 10);
    });
    it("check visit by non-admin", function () {
        cy.intercept("POST", "".concat(Cypress.env("VITE_API_URL"), "/api/auth/refresh-token"), { fixture: "auth-professor-response.json" });
        cy.intercept("GET", "".concat(Cypress.env("VITE_API_URL"), "/api/users/123/related-laboratories"), {
            fixture: "laboratories-response.json",
        });
        cy.get("ul[id='laboratories-list']").children().should("have.length", 10);
    });
    it("check list contents", function () {
        cy.intercept("POST", "".concat(Cypress.env("VITE_API_URL"), "/api/auth/refresh-token"), { fixture: "auth-admin-response.json" });
        cy.intercept("GET", "".concat(Cypress.env("VITE_API_URL"), "/api/laboratories/simplified"), {
            fixture: "laboratories-response.json",
        });
        cy.get("li[id='1']")
            .contains("Laboratory A")
            .should("have.attr", "href", "/reports/1");
        cy.get("li[id='2']")
            .contains("Laboratory B")
            .should("have.attr", "href", "/reports/2");
        cy.get("li[id='3']")
            .contains("Laboratory C")
            .should("have.attr", "href", "/reports/3");
        cy.get("li[id='4']")
            .contains("Laboratory D")
            .should("have.attr", "href", "/reports/4");
        cy.get("li[id='5']")
            .contains("Laboratory E")
            .should("have.attr", "href", "/reports/5");
    });
});
