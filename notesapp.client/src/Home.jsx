/* eslint-disable react/prop-types */
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import './App.css';
import {
    Form,
    FormGroup,
    Input,
    Row,
    Col,
    Card as ReactstrapCard, 
    CardTitle,
    CardText,
    ButtonGroup,
    Button,
} from 'reactstrap';
import NavbarComponent from './NavbarComponent';

function CustomCard(props) {
    const handleDelete = () => {
        const noteId = props.id;
        console.log("Note ID:", noteId);
        props.onDelete(noteId);
    };

    const navigate = useNavigate();

    const handleUpdateClick = (id) => {
        navigate(`/notes/update/${id}`);
    };

    return (
        <>
            <ReactstrapCard body>
                <CardTitle tag="h5">{props.title}</CardTitle>
                <CardText><b>{props.bodyText}</b></CardText>
                <ButtonGroup>
                    <Button style={{ margin: "5px" }} color="danger" onClick={handleDelete}>Delete</Button>
                    <Button style={{ margin: "5px" }} color="primary" onClick={() => handleUpdateClick(props.id)}>Update</Button>
                </ButtonGroup>
            </ReactstrapCard>
        </>
    );
}



const Home = () => {

    const [title, setTitle] = useState("");
    const [bodyText, setBodyText] = useState("");
    const [notes, setNotes] = useState([]);
    const [error, setError] = useState("");

    useEffect(() => {
        document.title = "Notes App";
        fetchData();
    }, []);

    function fetchData() {
        axios.get("https://localhost:7260/notes/getAll")
            .then((response) => {
                const sortedNotes = response.data.sort((a, b) => b.id - a.id);
                setNotes(sortedNotes);
            })
            .catch((error) => {
                setError(error.message);
            });
    }

    function handleDelete(id) {
        fetch(`https://localhost:7260/notes/${id}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
            },
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Failed to delete note');
                }
                setNotes(notes.filter(note => note.id !== id));
            })
            .catch(error => {
                console.error('Error deleting note:', error);
            });
    }


    function postRequest() {
        axios.post("https://localhost:7260/notes/Create", {
            title: title,
            bodyText: bodyText
        })
            .then((response) => {
                console.log(response.data)
                console.log(response.status);
                window.location.reload();
            })
            .catch((error) => {
                setError(error.message);
            });
    }

    function submitRequest(e) {
        e.preventDefault();

        if (!title.length || !bodyText.length) {
            setError("All fields must be filled!");
        } else {
            postRequest();
        }
    }

    return (
        <>
            <NavbarComponent />
            <div>
                <Form id="form_newnote" onSubmit={submitRequest}>
                    <h1 style={{ textAlign: "center", marginBottom: "10px" }}> New Note </h1>
                    <FormGroup>
                        <Input
                            type="text"
                            id="title"
                            value={title}
                            name="title"
                            placeholder="Title"
                            onChange={(e) => setTitle(e.target.value)}
                        />
                    </FormGroup>

                    <FormGroup>
                        <Input
                            id="body"
                            value={bodyText}
                            name="BodyText"
                            placeholder="Note..."
                            onChange={(e) => setBodyText(e.target.value)}
                            type="textarea"
                        />
                    </FormGroup>

                    <FormGroup>
                        <Button color="primary" style={{ width: "100%" }} type="submit">CREATE</Button>
                    </FormGroup>
                </Form>

                <hr className="hr" />

                {error.length ? <span> {error} </span> : null}

                <Row>
                    {notes.map((note, index) => (
                        <Col xs="12" sm="6" md="4" lg="3" key={index} style={{ margin: "0 px 10px", padding: "30px" }}>
                            <CustomCard
                                id={note.id}
                                title={note.title}
                                bodyText={note.bodyText}
                                onDelete={handleDelete}
                            />
                        </Col>
                    ))}
                </Row>

            </div>
        </>
    )
}

export default Home;