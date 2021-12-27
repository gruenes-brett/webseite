describe('gruenesbrett', () => {
    beforeEach(() => {
        cy.visit('http://testing');
    });

    it('allows me to add an event', () => {
        cy.get('nav a:contains(Veranstaltung eintragen)').click();
                cy.get('form#edit-event-form [type=submit]').click(); // Assert that we cannot submit yet

        // Fill mandatory fields
        cy.get('#inputName').type('Franz')
        cy.get('form#edit-event-form [type=submit]').click(); // Assert that we cannot submit yet
        cy.get('#inputEmail').type('cypress-gruenesbrett@bund-dresden.de')
        cy.get('form#edit-event-form [type=submit]').click(); // Assert that we cannot submit yet
        cy.get('#inputTitle').type('Vortrag zur One-For-One-Regel')
        cy.get('form#edit-event-form [type=submit]').click(); // Assert that we cannot submit yet
        cy.get('#inputPrivacy').check()
        cy.get('form#edit-event-form [type=submit]').click(); // Assert that we cannot submit yet

        cy.get('header').compareSnapshot('nav');
    });
});
