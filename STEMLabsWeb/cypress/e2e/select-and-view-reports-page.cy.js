describe("select-and-view-reports-page", function () {
    beforeEach(function () {
        localStorage.setItem("refreshToken", "refreshToken123");
        cy.visit("http://localhost:8080/reports/1/2/3");
        cy.intercept("POST", "".concat(Cypress.env("VITE_API_URL"), "/api/auth/refresh-token"), { fixture: "auth-admin-response.json" });
        cy.intercept("GET", "".concat(Cypress.env("VITE_API_URL"), "/api/laboratory-sessions/2/participants/3/reports"), {
            fixture: "reports-list-response.json",
        });
    });
    it("check report tabs list", function () {
        cy.get("[id='reports-list']").children().should("have.length", 3);
        var timeZoneOffset = new Date().getTimezoneOffset();
        var UTCOffset = (timeZoneOffset < 0 ? "+" : "-") + timeZoneOffset / -60;
        cy.get("[id='1']").contains("05/04/2025, ".concat((14 + timeZoneOffset / -60) % 24, ":48:00 UTC") + UTCOffset);
        cy.get("[id='2']").contains("06/05/2026, ".concat((15 + timeZoneOffset / -60) % 24, ":49:01 UTC") + UTCOffset);
        cy.get("[id='3']").contains("07/06/2027, ".concat((16 + timeZoneOffset / -60) % 24, ":50:02 UTC") + UTCOffset);
    });
    it("check report content", function () {
        cy.get("[id='1']").click();
        cy.intercept("GET", "".concat(Cypress.env("VITE_API_URL"), "/api/laboratory-reports/1"), {
            fixture: "report-content-response.json",
        });
        cy.get("[id='submitter-full-name']").should("have.value", "John Doe");
        cy.get("[id='step-1-label']")
            .contains("Step 1")
            .should("have.css", "color", "rgb(46, 125, 50)"); // green color
        cy.get("[id='step-1']").contains("Verify the system is operational");
        cy.get("[id='step-2-label']")
            .contains("Step 2")
            .should("have.css", "color", "rgb(211, 47, 47)"); // red color
        cy.get("[id='step-2']").contains("Ensure all components are functioning correctly");
        cy.get("[id='step-3-label']")
            .contains("Step 3")
            .should("have.css", "color", "rgb(46, 125, 50)");
        cy.get("[id='step-3']").contains("Confirm the system meets all operational standards");
        cy.get("img[id='observations-image']").should("have.attr", "src", "https://placehold.co/600x400.png");
    });
});
