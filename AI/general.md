# General
## Skills
- Skills are repeatable 
- All AI configs is under `~/.config` in your computer
- All AI skills accessible by any models are put in `~/.agents/skills` in your computer
## Agents.md vs. .instruction files
- They are the same; `agents.md` are used cross-models, while `.instruction` files are for Copilot only
- For Rider Github copilot, put your `instruction` files in `/Users/aldo.sebastian/.config/github-copilot/intellij/` so it is usable in any repo
## Hooks
- Commands that will run (deterministically i.e. for sure) on certain points during AI answering your questions
- No cross-model setup is available yet at time of writing this doc
- For Copilot: `https://docs.github.com/en/copilot/concepts/agents/cloud-agent/about-hooks`
- Useful hooks:
```copilot-example
{
  "version": 1,
  "hooks": {
    "agentStop": [
      {
        "type": "command",
        "bash": "osascript -e 'display notification \"AI is done!\" with title \"Hey\"'"
      }
    ]
  }
}

```

# Costs of Copilot
- https://github.com/SachiHarshitha/copilot-usage to check your copilot usage by scanning your logs. When installing, put `README.md` on the `apps/cli` folder
## Request vs. usage based billing
- Before 1 June 2026, `$39` (for enterprise) includes 1000 requests
  - Multipliers like Opus 4.6 (3x) means each request using Opus 4.6 is counted as 3 requests
- After 1 June 2026, `$39` includes 3900 AI credits
  - 1 AI credit = `$0.01` USD
  - The usage is following this (table)[https://docs.github.com/en/copilot/reference/copilot-billing/models-and-pricing#pricing-tables]
  - Example calculation: (here)[https://docs.google.com/document/d/1nyymCr3jOVCT9hE5ttGL_Q8TKBgEcs1SlqRnLSJDuvw/edit?usp=sharing]

## How to reduce costs
- Use models with cache inputs and outputs
  - Since this means that subsequent requests who uses the same resource (e.g. a follow up question to explaining a codebase) reduces the token costs for those words for that request.
  - Although, cache inputs means on the first request you need to write the context(s) to a cache, and that may incur `cache write` costs (this only applies for Anthropic harness, but may be the case for
  copilot in the future)
- Quick mental model
  - 75000 words in input (prompt + all sources used) using Claude Opus 4.6  ~ 100000 tokens => $0.5 => 50 AI credits
  - 800 to 1000 lines of code ~ 100000 tokens
  - 1 word ~ 0.75 tokens
  - 1-2 sentences ~ 30 tokens
  - 1 paragraph ~ 150 tokens
- So then if every request i have a 100000 token input then 0.5 per request at least = 50 ai credits, less than 80 requests I would've exhausted everything for enterprise plan (3900 AI Credits)
  - If you include the initial Cache Write fee ($1.125$) or any output tokens, that number drops even lower
  - Before June, you were encouraged to dump as much context as possible into the chat because it was "free." 
  - Now, you are financially penalized for including large files that aren't strictly necessary.
  - So TLDR/rule of thumb: in new pricing model you must be more careful on the no. of stuff you put in as context! Only put in necessary stuff
    - This has always been the best-practice anyway (to avoid hitting the context window limit), but now we have an extra (monetary) reason, on top of performance reasons.
    - This also means though, we have to use less hooks/skills/extra helpers on our chat session to keep costs low :( .
  - Prompt Caching: This is the only way to "beat" the system. If you stay in one chat session and ask 20 questions about the same 100k file, your total cost is only about $1.60. In the old system, those 20 questions would have used 2% of your entire monthly quota.
- So, if your workflow is "One complex question per project," this new pricing (usage based vs. request based) is significantly more expensive.
- If your workflow is "Long, iterative debugging sessions on the same file," the pricing stays roughly the same or even gets slightly better thanks to caching.
