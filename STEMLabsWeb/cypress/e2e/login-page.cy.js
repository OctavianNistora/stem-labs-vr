describe("login", function () {
    beforeEach(function () {
        cy.visit("http://localhost:8080/login");
    });
    it("type into username field", function () {
        cy.get("input[id='username-field']")
            .type("testuser")
            .should("have.value", "testuser");
    });
    it("type into password field", function () {
        cy.get("input[id='password-field']")
            .type("testpassword")
            .should("have.value", "testpassword");
    });
    it("click login button", function () {
        cy.get("input[id='username-field']").type("testuser");
        cy.get("input[id='password-field']").type("testpassword");
        cy.get("button[id='login-button']").click();
        cy.intercept("POST", "".concat(Cypress.env("VITE_API_URL"), "/api/auth/session"), function (req) {
            expect(req.body.username).to.equal("testuser");
            expect(req.body.password).to.equal("testpassword");
            expect(req.body.respondWithRefreshToken).to.equal(true);
            req.reply({ fixture: "auth-student-response.json" });
        });
        cy.url().should("equal", "http://localhost:8080/");
    });
});
