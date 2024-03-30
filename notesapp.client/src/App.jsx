import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Home from './Home';
import UpdateNotePage from './UpdateNoteText';

function App() {
    return (
        <>
        <Router>
            <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/notes/update/:id" element={<UpdateNotePage />} />
            </Routes>
            </Router>
        </>

    );
}

export default App;
