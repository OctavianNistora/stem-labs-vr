describe("home-page", () => {
  beforeEach(() => {
    localStorage.setItem("refreshToken", "refreshToken123");
    cy.visit("http://localhost:2077");
    cy.intercept(
      "POST",
      `${Cypress.env("VITE_API_URL")}/api/auth/refresh-token`,
      { fixture: "auth-admin-response.json" },
    );
  });

  it("check drawer hyperlink buttons", () => {
    cy.get("[id='home-page-drawer-button']").should("have.attr", "href", "/");
    cy.get("[id='view-reports-drawer-button']").should(
      "have.attr",
      "href",
      "/reports",
    );
    cy.get("[id='manage-laboratories-drawer-button']").should(
      "have.attr",
      "href",
      "/laboratories",
    );
    cy.get("[id='manage-users-drawer-button']").should(
      "have.attr",
      "href",
      "/manage-users",
    );
    cy.get("[id='profile-drawer-button']").should(
      "have.attr",
      "href",
      "/profile",
    );
    cy.get("[id='logout-drawer-button']");
  });

  it("check homepage buttons", () => {
    cy.get("[id='home-page-homepage-button']").should("have.attr", "href", "/");
    cy.get("[id='view-reports-homepage-button']").should(
      "have.attr",
      "href",
      "/reports",
    );
    cy.get("[id='manage-laboratories-homepage-button']").should(
      "have.attr",
      "href",
      "/laboratories",
    );
    cy.get("[id='manage-users-homepage-button']").should(
      "have.attr",
      "href",
      "/manage-users",
    );
    cy.get("[id='profile-homepage-button']").should(
      "have.attr",
      "href",
      "/profile",
    );
  });
});
