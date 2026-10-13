import type { GameCardGame } from "../../types/game";
import { DifficultyBadge, Tag } from "../ui/Badges";
import { Link } from "react-router-dom";

type GameCardProps = {
	game: GameCardGame;
	isFavorite?: boolean;
	onToggleFavorite?: () => void;
};

export default function GameCard({ game, isFavorite = false, onToggleFavorite }: GameCardProps) {
	const rating = game.rating === null ? "--" : game.rating.toFixed(1);
	const reviewCount = game.reviewCount === null ? "No analyzed reviews" : `${game.reviewCount.toLocaleString()} analyzed reviews`;
	const positivePct = game.positivePct === null ? null : Math.round(game.positivePct);

	return (
	<article className="game-card game-card-link">
		<Link to={`/games/${game.id}`} className="game-card-link-main" aria-label={`View ${game.title}`}>
			<div className="game-card-cover">
				<img src={game.coverImage} alt="" />
				<div className="game-card-difficulty">
					<DifficultyBadge difficulty={game.difficulty} />
				</div>
				<div className="game-card-cover-gradient" />
				<div className="game-card-rating">
					<span>{rating}</span>
					<small>/ 5</small>
				</div>
			</div>

			<div className="game-card-content">
				<div>
					<h3>{game.title}</h3>
					<p>{reviewCount}</p>
				</div>

				<div className="game-card-meta">
					<span>{game.minPlayers}-{game.maxPlayers} players</span>
					<span>{game.minPlayTime}-{game.maxPlayTime} min</span>
				</div>

				{positivePct === null ? (
					<p className="game-card-no-insight">AI insight has not been generated yet.</p>
				) : (
					<div className="sentiment-row">
						<div className="sentiment-bar" aria-hidden="true">
							<div style={{ width: `${positivePct}%` }} />
						</div>
						<span>{positivePct}% positive</span>
					</div>
				)}

				{game.tags.length > 0 && (
					<div className="game-card-tags">
						{game.tags.map((tag) => (
							<Tag key={tag} label={tag} />
						))}
					</div>
				)}

				<div className="game-card-footer">
					<span>Explore game <span aria-hidden="true">→</span></span>
				</div>
			</div>
		</Link>
		{onToggleFavorite && (
			<button className="game-favorite-button" type="button" aria-label={isFavorite ? `Remove ${game.title} from favorites` : `Add ${game.title} to favorites`} onClick={onToggleFavorite}>
				{isFavorite ? '♥' : '♡'}
			</button>
		)}
	</article>
	);
}