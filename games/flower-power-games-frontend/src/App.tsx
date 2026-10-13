import {
    BrowserRouter,
    Navigate,
    Route,
    Routes,
} from 'react-router-dom';

import Login from './pages/Login';
import Discover from "./pages/Discover";

function App() {
    return (
        <BrowserRouter>
            <Routes>
        <Route
                    path= "/login"
    element = {< Login />}
                />

        <Route path="/discover" element={<Discover />} />   

    < Route
path = "*"
element = {
                        < Navigate
to = "/login"
replace
    />
                    }
                />
    </Routes>
    </BrowserRouter>
    );
}

export default App;