import { Navbar, Container } from 'react-bootstrap';

function NavbarComponent() {
    return (
        <Container>
            <Navbar expand="lg" bg="body-tertiary">
                <Container fluid>
                    <Navbar.Brand href="/">Navbar</Navbar.Brand>
                </Container>
            </Navbar>
        </Container>
    );
}

export default NavbarComponent;