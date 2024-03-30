import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Form, FormGroup, Input, Button } from 'reactstrap';
import axios from 'axios';
import './App.css';
import NavbarComponent from './NavbarComponent';

function UpdateNotePage() {
    const { id } = useParams();
    const [note, setNote] = useState({ title: '', bodyText: '' });
    const navigate = useNavigate();

    useEffect(() => {
        document.title = "Update Note";
        fetchNoteDetails();
    }, []);

    const fetchNoteDetails = async () => {
        try {
            const response = await axios.get(`https://localhost:7260/notes/${id}`);
            console.log(response);
            const { title, bodyText } = response.data;
            setNote({ title, bodyText });
            console.log(note);
        } catch (error) {
            console.error('Error fetching note details:', error);
        }
    };


    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setNote(prevNote => ({
            ...prevNote,
            [name]: value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.put(`https://localhost:7260/notes/update/${id}`, note);
            console.log('Note updated successfully:', response.data);
            navigate('/');
        } catch (error) {
            console.error('Error updating note:', error);
            if (error.response) {
                console.error('Server responded with:', error.response.data);
            }
        }
    };


    return (
        <>
        <NavbarComponent />
        <div>
            <Form id="form_newnote" onSubmit={handleSubmit}>
                <h1 style={{ textAlign: "center", marginBottom: "10px" }}> Update Note </h1>
                <FormGroup>
                    <Input
                        style={{ color: "black" }}
                        type="text" name="title"
                        value={note.title}
                        onChange={handleInputChange}
                    />
                </FormGroup>
                <FormGroup>
                    <Input
                        name="bodyText"
                        value={note.bodyText}
                        onChange={handleInputChange}
                        style={{ color: "black" }}
                    />
                </FormGroup>

                <FormGroup>
                    <Button color="primary" style={{ width: "100%" }} type="submit">UPDATE</Button>
                </FormGroup>
            </Form>
            </div>
        </>
    );
}

export default UpdateNotePage;
