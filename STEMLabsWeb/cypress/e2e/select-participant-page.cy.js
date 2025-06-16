describe("select-participant-page", function () {
    beforeEach(function () {
        localStorage.setItem("refreshToken", "refreshToken123");
        cy.visit("http://localhost:8080/reports/1/2");
    });
    it("check visit by student", function () {
        cy.intercept("POST", "".concat(Cypress.env("VITE_API_URL"), "/api/auth/refresh-token"), { fixture: "auth-student-response.json" });
        cy.url().should("equal", "http://localhost:8080/reports/1");
    });
    it("check visit by non-student", function () {
        cy.intercept("POST", "".concat(Cypress.env("VITE_API_URL"), "/api/auth/refresh-token"), { fixture: "auth-admin-response.json" });
        cy.intercept("GET", "".concat(Cypress.env("VITE_API_URL"), "/api/laboratory-sessions/2/participants"), {
            fixture: "participants-response.json",
        });
        cy.get("ul[id='participants-list']").children().should("have.length", 10);
    });
    it("check list contents", function () {
        cy.intercept("POST", "".concat(Cypress.env("VITE_API_URL"), "/api/auth/refresh-token"), { fixture: "auth-admin-response.json" });
        cy.intercept("GET", "".concat(Cypress.env("VITE_API_URL"), "/api/laboratory-sessions/2/participants"), {
            fixture: "participants-response.json",
        });
        var timeZoneOffset = new Date().getTimezoneOffset();
        var UTCOffset = (timeZoneOffset < 0 ? "+" : "-") + timeZoneOffset / -60;
        cy.get("li[id='1']")
            .contains("Participant 1")
            .contains("05/04/2025, ".concat((14 + timeZoneOffset / -60) % 24, ":48:00 UTC") + UTCOffset);
        cy.get("li[id='2']")
            .contains("Participant 2")
            .contains("06/05/2026, ".concat((15 + timeZoneOffset / -60) % 24, ":49:01 UTC") + UTCOffset);
        cy.get("li[id='3']")
            .contains("Participant 3")
            .contains("07/06/2027, ".concat((16 + timeZoneOffset / -60) % 24, ":50:02 UTC") + UTCOffset);
        cy.get("li[id='4']")
            .contains("Participant 4")
            .contains("08/07/2028, ".concat((17 + timeZoneOffset / -60) % 24, ":51:03 UTC") + UTCOffset);
        cy.get("li[id='5']")
            .contains("Participant 5")
            .contains("09/08/2029, ".concat((18 + timeZoneOffset / -60) % 24, ":52:04 UTC") + UTCOffset);
    });
});
